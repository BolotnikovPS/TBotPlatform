namespace TBotPlatform.Contracts.Bots.ChatUpdate.ChatMessages;

public class ChatShippingAddress(
    string countryCode,
    string state,
    string city,
    string streetLine1,
    string streetLine2,
    string postCode
    )
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code
    /// </summary>
    public string CountryCode { get; } = countryCode;

    /// <summary>
    /// State, if applicable
    /// </summary>
    public string State { get; } = state;

    /// <summary>
    /// City
    /// </summary>
    public string City { get; } = city;

    /// <summary>
    /// First line for the address
    /// </summary>
    public string StreetLine1 { get; } = streetLine1;

    /// <summary>
    /// Second line for the address
    /// </summary>
    public string StreetLine2 { get; } = streetLine2;

    /// <summary>
    /// Address post code
    /// </summary>
    public string PostCode { get; } = postCode;
}