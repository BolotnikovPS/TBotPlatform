namespace TBotPlatform.Contracts.Bots.Locations;

public class TelegramLocation(
    double longitude,
    double latitude,
    double? horizontalAccuracy,
    int? livePeriod,
    int? heading,
    int? proximityAlertRadius
    )
{
    /// <summary>
    /// Longitude as defined by sender
    /// </summary>
    public double Longitude { get; } = longitude;

    /// <summary>
    /// Latitude as defined by sender
    /// </summary>
    public double Latitude { get; } = latitude;

    /// <summary>
    /// Optional. The radius of uncertainty for the location, measured in meters; 0-1500
    /// </summary>
    public double? HorizontalAccuracyOrNull { get; } = horizontalAccuracy;

    /// <summary>
    /// Optional. Time relative to the message sending date, during which the location can be updated, in seconds. For active live locations only.
    /// </summary>
    public int? LivePeriodOrNull { get; } = livePeriod;

    /// <summary>
    /// Optional. The direction in which user is moving, in degrees; 1-360. For active live locations only.
    /// </summary>
    public int? HeadingOrNull { get; } = heading;

    /// <summary>
    /// Optional. Maximum distance for proximity alerts about approaching another chat member, in meters. For sent live locations only.
    /// </summary>
    public int? ProximityAlertRadiusOrNull { get; } = proximityAlertRadius;
}