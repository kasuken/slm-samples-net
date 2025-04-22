/// <summary>
/// Represents a horoscope prediction with detailed information
/// </summary>
public class HoroscopePrediction
{
    /// <summary>
    /// The zodiac sign for this horoscope
    /// </summary>
    public string Sign { get; set; }

    /// <summary>
    /// Detailed horoscope information for different aspects of life
    /// </summary>
    public HoroscopeDetails Horoscope { get; set; }

    /// <summary>
    /// The zodiac sign that is most compatible with this sign for the day
    /// </summary>
    public string Compatibility { get; set; }

    /// <summary>
    /// One or two words describing the overall mood for the day
    /// </summary>
    public string Mood { get; set; }
}

/// <summary>
/// Contains detailed horoscope information for different aspects of life
/// </summary>
public class HoroscopeDetails
{
    /// <summary>
    /// General outlook for the day
    /// </summary>
    public string General { get; set; }

    /// <summary>
    /// Love and relationship outlook for the day
    /// </summary>
    public string Love { get; set; }

    /// <summary>
    /// Career and work outlook for the day
    /// </summary>
    public string Career { get; set; }

    /// <summary>
    /// Health and wellness outlook for the day
    /// </summary>
    public string Health { get; set; }

    /// <summary>
    /// Lucky number for the day (1-9)
    /// </summary>
    public int LuckyNumber { get; set; }
}
