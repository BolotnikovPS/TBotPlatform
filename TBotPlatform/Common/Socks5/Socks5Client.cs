#nullable enable

using System.Net;
using System.Net.Sockets;
using System.Text;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Common.Socks5;

/// <summary>
/// Minimal SOCKS5 client (RFC 1928) with optional username/password authentication (RFC 1929).
/// Implemented from scratch so Telegram traffic can be routed through a SOCKS5 proxy
/// without any third-party dependency. It is consumed as the
/// <see cref="SocketsHttpHandler.ConnectCallback"/>, which receives the already-resolved
/// <see cref="SocketsHttpConnectionContext"/> and returns a tunneled stream.
/// </summary>
internal static class Socks5Client
{
    private const byte Version = 0x05;
    private const byte MethodNoAuth = 0x00;
    private const byte MethodUserPass = 0x02;
    private const byte MethodNoAcceptable = 0xFF;

    private const byte CommandConnect = 0x01;
    private const byte AddressTypeIpv4 = 0x01;
    private const byte AddressTypeDomain = 0x03;
    private const byte AddressTypeIpv6 = 0x04;

    public static async ValueTask<Stream> ConnectAsync(
        SocketsHttpConnectionContext context,
        TBotSettingProxy proxy,
        CancellationToken cancellationToken)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };

        try
        {
            await socket.ConnectAsync(proxy.Host, proxy.Port, cancellationToken).ConfigureAwait(false);

            var stream = new NetworkStream(socket, ownsSocket: true);

            try
            {
                await HandshakeAsync(stream, proxy, cancellationToken).ConfigureAwait(false);
                await ConnectAsync(stream, context.DnsEndPoint, cancellationToken).ConfigureAwait(false);

                return stream;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    private static async Task HandshakeAsync(Stream stream, TBotSettingProxy proxy, CancellationToken cancellationToken)
    {
        var hasCredentials = !string.IsNullOrEmpty(proxy.Username);

        // Greeting: VER, NMETHODS, METHODS
        var greeting = hasCredentials
            ? new byte[] { Version, 0x02, MethodNoAuth, MethodUserPass }
            : new byte[] { Version, 0x01, MethodNoAuth };

        await stream.WriteAsync(greeting, cancellationToken).ConfigureAwait(false);

        var selection = await ReadExactlyAsync(stream, 2, cancellationToken).ConfigureAwait(false);

        if (selection[0] != Version || selection[1] == MethodNoAcceptable)
        {
            throw new IOException("The SOCKS5 proxy rejected all authentication methods.");
        }

        if (selection[1] == MethodUserPass)
        {
            await AuthenticateAsync(stream, proxy, cancellationToken).ConfigureAwait(false);
        }
        else if (selection[1] != MethodNoAuth)
        {
            throw new IOException($"The SOCKS5 proxy selected an unsupported authentication method 0x{selection[1]:X2}.");
        }
    }

    private static async Task AuthenticateAsync(Stream stream, TBotSettingProxy proxy, CancellationToken cancellationToken)
    {
        var username = Encoding.UTF8.GetBytes(proxy.Username ?? string.Empty);
        var password = Encoding.UTF8.GetBytes(proxy.Password ?? string.Empty);

        if (username.Length > 255 || password.Length > 255)
        {
            throw new ArgumentException("SOCKS5 username/password must not exceed 255 bytes.", nameof(proxy));
        }

        // RFC 1929: VER=0x01, ULEN, UNAME, PLEN, PASSWD
        var buffer = new byte[3 + username.Length + password.Length];
        buffer[0] = 0x01;
        buffer[1] = (byte)username.Length;
        username.CopyTo(buffer, 2);
        buffer[2 + username.Length] = (byte)password.Length;
        password.CopyTo(buffer, 3 + username.Length);

        await stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

        var reply = await ReadExactlyAsync(stream, 2, cancellationToken).ConfigureAwait(false);

        if (reply[1] != 0x00)
        {
            throw new IOException("The SOCKS5 proxy rejected the username/password credentials.");
        }
    }

    private static async Task ConnectAsync(Stream stream, DnsEndPoint endpoint, CancellationToken cancellationToken)
    {
        var host = endpoint.Host;
        var port = endpoint.Port;

        // CONNECT request: VER, CMD=0x01, RSV=0x00, ATYP, address, port (big-endian)
        var request = BuildConnectRequest(host, port);

        await stream.WriteAsync(request, cancellationToken).ConfigureAwait(false);

        var reply = await ReadExactlyAsync(stream, 4, cancellationToken).ConfigureAwait(false);

        if (reply[0] != Version)
        {
            throw new IOException("The SOCKS5 proxy returned an invalid reply version.");
        }

        if (reply[1] != 0x00)
        {
            throw new IOException($"The SOCKS5 proxy failed to connect: {DescribeReply(reply[1])}.");
        }

        var addressLength = reply[3] switch
        {
            AddressTypeIpv4 => 4,
            AddressTypeIpv6 => 16,
            AddressTypeDomain => await ReadByteAsync(stream, cancellationToken).ConfigureAwait(false),
            _ => throw new IOException($"The SOCKS5 proxy returned an unknown address type 0x{reply[3]:X2}."),
        };

        // Drain the bound address and port; the tunnel is established from here on.
        await ReadExactlyAsync(stream, addressLength + 2, cancellationToken).ConfigureAwait(false);
    }

    private static byte[] BuildConnectRequest(string host, int port)
    {
        if (IPAddress.TryParse(host, out var ip))
        {
            var address = ip.GetAddressBytes();

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                var buffer = new byte[4 + 16 + 2];
                buffer[0] = Version;
                buffer[1] = CommandConnect;
                buffer[2] = 0x00;
                buffer[3] = AddressTypeIpv6;
                address.CopyTo(buffer, 4);
                buffer[^2] = (byte)(port >> 8);
                buffer[^1] = (byte)port;
                return buffer;
            }

            var ipv4 = new byte[4 + 4 + 2];
            ipv4[0] = Version;
            ipv4[1] = CommandConnect;
            ipv4[2] = 0x00;
            ipv4[3] = AddressTypeIpv4;
            address.CopyTo(ipv4, 4);
            ipv4[^2] = (byte)(port >> 8);
            ipv4[^1] = (byte)port;
            return ipv4;
        }

        // Pass the domain name to the proxy so it performs remote DNS resolution.
        var domain = Encoding.UTF8.GetBytes(host);

        if (domain.Length > 255)
        {
            throw new ArgumentException("The SOCKS5 destination host must not exceed 255 bytes.", nameof(host));
        }

        var result = new byte[4 + 1 + domain.Length + 2];
        result[0] = Version;
        result[1] = CommandConnect;
        result[2] = 0x00;
        result[3] = AddressTypeDomain;
        result[4] = (byte)domain.Length;
        domain.CopyTo(result, 5);
        result[^2] = (byte)(port >> 8);
        result[^1] = (byte)port;
        return result;
    }

    private static string DescribeReply(byte code)
        => code switch
        {
            0x01 => "general failure",
            0x02 => "connection not allowed by the ruleset",
            0x03 => "network unreachable",
            0x04 => "host unreachable",
            0x05 => "connection refused",
            0x06 => "TTL expired",
            0x07 => "command not supported",
            0x08 => "address type not supported",
            _ => $"unknown error 0x{code:X2}",
        };

    private static async Task<byte> ReadByteAsync(Stream stream, CancellationToken cancellationToken)
    {
        var buffer = new byte[1];
        var read = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

        if (read != 1)
        {
            throw new IOException("The SOCKS5 proxy closed the connection unexpectedly.");
        }

        return buffer[0];
    }

    private static async Task<byte[]> ReadExactlyAsync(Stream stream, int count, CancellationToken cancellationToken)
    {
        var buffer = new byte[count];
        var offset = 0;

        while (offset < count)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(offset, count - offset), cancellationToken).ConfigureAwait(false);

            if (read == 0)
            {
                throw new IOException("The SOCKS5 proxy closed the connection unexpectedly.");
            }

            offset += read;
        }

        return buffer;
    }
}
