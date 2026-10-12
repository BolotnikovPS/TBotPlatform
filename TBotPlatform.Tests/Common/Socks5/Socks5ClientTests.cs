using System.Net;
using System.Net.Sockets;
using System.Text;
using TBotPlatform.Common.Socks5;
using TBotPlatform.Contracts.Bots.Config;

namespace TBotPlatform.Tests.Common.Socks5;

/// <summary>
/// Tests the minimal SOCKS5 client (RFC 1928 / RFC 1929) used as the
/// <see cref="SocketsHttpHandler.ConnectCallback"/> for proxied Telegram traffic.
/// </summary>
[TestFixture]
public class Socks5ClientTests
{
    private const string DestinationHost = "api.telegram.org";
    private const int DestinationPort = 443;

    [Test]
    public void BuildConnectRequest_ForIpv4Address_UsesAddressTypeIpv4()
    {
        var request = Socks5Client.BuildConnectRequest("8.8.8.8", 1080);

        Assert.That(request, Has.Length.EqualTo(4 + 4 + 2));
        Assert.That(request[0], Is.EqualTo(0x05));
        Assert.That(request[1], Is.EqualTo(0x01)); // CONNECT
        Assert.That(request[2], Is.EqualTo(0x00)); // RSV
        Assert.That(request[3], Is.EqualTo(0x01)); // IPv4
        Assert.That(request[4..8], Is.EqualTo(new byte[] { 8, 8, 8, 8 }));
        Assert.That(request[8], Is.EqualTo(0x04)); // port high (1080 = 0x0438)
        Assert.That(request[9], Is.EqualTo(0x38)); // port low
    }

    [Test]
    public void BuildConnectRequest_ForIpv6Address_UsesAddressTypeIpv6()
    {
        var request = Socks5Client.BuildConnectRequest("::1", DestinationPort);

        Assert.That(request, Has.Length.EqualTo(4 + 16 + 2));
        Assert.That(request[3], Is.EqualTo(0x04)); // IPv6
        Assert.That(request[^2], Is.EqualTo(0x01)); // port high (443 = 0x01BB)
        Assert.That(request[^1], Is.EqualTo(0xBB)); // port low
    }

    [Test]
    public void BuildConnectRequest_ForDomain_DelegatesDnsResolutionToTheProxy()
    {
        var request = Socks5Client.BuildConnectRequest(DestinationHost, DestinationPort);

        Assert.That(request[3], Is.EqualTo(0x03)); // domain
        Assert.That(request[4], Is.EqualTo(DestinationHost.Length));
        Assert.That(Encoding.UTF8.GetString(request[5..(5 + DestinationHost.Length)]), Is.EqualTo(DestinationHost));
        Assert.That(request[^2], Is.EqualTo(0x01)); // port high
        Assert.That(request[^1], Is.EqualTo(0xBB)); // port low
    }

    [Test]
    public void BuildConnectRequest_ForOverlongDomain_Throws()
    {
        var host = new string('a', 256);

        Assert.Throws<ArgumentException>(() => Socks5Client.BuildConnectRequest(host, DestinationPort));
    }

    [TestCase(0x01, "general failure")]
    [TestCase(0x02, "connection not allowed by the ruleset")]
    [TestCase(0x03, "network unreachable")]
    [TestCase(0x04, "host unreachable")]
    [TestCase(0x05, "connection refused")]
    [TestCase(0x06, "TTL expired")]
    [TestCase(0x07, "command not supported")]
    [TestCase(0x08, "address type not supported")]
    public void DescribeReply_ForKnownCodes_ReturnsHumanReadableText(byte code, string expected)
        => Assert.That(Socks5Client.DescribeReply(code), Is.EqualTo(expected));

    [Test]
    public void DescribeReply_ForUnknownCode_ReturnsHexCode()
        => Assert.That(Socks5Client.DescribeReply(0x42), Is.EqualTo("unknown error 0x42"));

    [Test]
    public async Task ConnectThroughStreamAsync_WithNoAuthProxy_EstablishesTunnelAndDelegatesDomainDns()
    {
        var server = StartServer(out var port, AuthMode.NoAuth);
        await using var stream = await ConnectClientAsync(port);
        var endpoint = new DnsEndPoint(DestinationHost, DestinationPort);

        await Socks5Client.ConnectThroughStreamAsync(stream, endpoint, CreateProxy(port), CancellationToken.None);
        var capture = await server;

        Assert.That(capture.Methods, Is.EqualTo(new byte[] { 0x00 }));
        Assert.That(capture.Atyp, Is.EqualTo(0x03));
        Assert.That(Encoding.UTF8.GetString(capture.Address), Is.EqualTo(DestinationHost));
        Assert.That(capture.Port, Is.EqualTo(DestinationPort));
        Assert.That(capture.User, Is.Null);
    }

    [Test]
    public async Task ConnectThroughStreamAsync_WithCredentials_PerformsRfc1929Authentication()
    {
        var server = StartServer(out var port, AuthMode.UserPass);
        await using var stream = await ConnectClientAsync(port);
        var endpoint = new DnsEndPoint(DestinationHost, DestinationPort);

        await Socks5Client.ConnectThroughStreamAsync(stream, endpoint, CreateProxy(port, "alice", "s3cret"), CancellationToken.None);
        var capture = await server;

        Assert.That(capture.Methods, Is.EqualTo(new byte[] { 0x00, 0x02 }));
        Assert.That(capture.User, Is.EqualTo("alice"));
        Assert.That(capture.Password, Is.EqualTo("s3cret"));
    }

    [Test]
    public async Task ConnectThroughStreamAsync_WhenProxyRejectsTheConnect_ThrowsIOException()
    {
        var server = StartServer(out var port, AuthMode.RejectConnect);
        await using var stream = await ConnectClientAsync(port);
        var endpoint = new DnsEndPoint(DestinationHost, DestinationPort);

        var exception = await Assert.ThrowsAsync<IOException>(async () =>
            await Socks5Client.ConnectThroughStreamAsync(stream, endpoint, CreateProxy(port), CancellationToken.None));

        Assert.That(exception.Message, Does.Contain("connection refused"));

        await server;
    }

    private static TBotSettingProxy CreateProxy(int port, string? username = null, string? password = null)
        => new() { Host = "127.0.0.1", Port = port, Username = username, Password = password };

    private static async Task<NetworkStream> ConnectClientAsync(int port)
    {
        var client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, port);

        return client.GetStream();
    }

    private static Task<ServerCapture> StartServer(out int port, AuthMode mode = AuthMode.NoAuth)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        port = ((IPEndPoint)listener.LocalEndpoint).Port;

        return ServeAsync(listener, mode);
    }

    private static async Task<ServerCapture> ServeAsync(TcpListener listener, AuthMode mode, CancellationToken ct = default)
    {
        try
        {
            using var client = await listener.AcceptTcpClientAsync(ct);
            await using var stream = client.GetStream();

            await ReadByteAsync(stream, ct); // VER
            var methodCount = await ReadByteAsync(stream, ct); // NMETHODS
            var methods = await ReadExactlyAsync(stream, methodCount, ct);

            var selected = mode == AuthMode.UserPass ? (byte)0x02 : (byte)0x00;
            await stream.WriteAsync(new byte[] { 0x05, selected }, ct);

            string? user = null;
            string? password = null;

            if (mode == AuthMode.UserPass)
            {
                await ReadByteAsync(stream, ct); // VER

                var userLength = await ReadByteAsync(stream, ct);
                user = Encoding.UTF8.GetString(await ReadExactlyAsync(stream, userLength, ct));

                var passwordLength = await ReadByteAsync(stream, ct);
                password = Encoding.UTF8.GetString(await ReadExactlyAsync(stream, passwordLength, ct));

                await stream.WriteAsync(new byte[] { 0x01, 0x00 }, ct);
            }

            var header = await ReadExactlyAsync(stream, 4, ct); // VER, CMD, RSV, ATYP
            var atyp = header[3];
            var addressLength = atyp switch
            {
                0x01 => 4,
                0x04 => 16,
                0x03 => await ReadByteAsync(stream, ct),
                _ => throw new InvalidOperationException($"Unexpected address type 0x{atyp:X2}."),
            };

            var address = await ReadExactlyAsync(stream, addressLength, ct);
            var portBytes = await ReadExactlyAsync(stream, 2, ct);

            var reply = mode == AuthMode.RejectConnect
                ? new byte[] { 0x05, 0x05, 0x00, 0x01, 0, 0, 0, 0, 0, 0 }
                : new byte[] { 0x05, 0x00, 0x00, 0x01, 0, 0, 0, 0, 0, 0 };

            await stream.WriteAsync(reply, ct);

            return new ServerCapture(methods, atyp, address, (portBytes[0] << 8) | portBytes[1], user, password);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task<byte> ReadByteAsync(Stream stream, CancellationToken ct)
    {
        var buffer = new byte[1];
        var read = await stream.ReadAsync(buffer, ct);

        return read == 1 ? buffer[0] : throw new EndOfStreamException();
    }

    private static async Task<byte[]> ReadExactlyAsync(Stream stream, int count, CancellationToken ct)
    {
        var buffer = new byte[count];
        var offset = 0;

        while (offset < count)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(offset, count - offset), ct);

            if (read == 0)
            {
                throw new EndOfStreamException();
            }

            offset += read;
        }

        return buffer;
    }

    private enum AuthMode
    {
        NoAuth,
        UserPass,
        RejectConnect,
    }

    private sealed record ServerCapture(
        byte[] Methods,
        byte Atyp,
        byte[] Address,
        int Port,
        string? User,
        string? Password);
}
