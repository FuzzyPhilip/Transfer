namespace Beinggs.Transfer.Extensions;


/// <summary>
/// Handy-dandy extension methods for numeric values.
/// </summary>
public static class NumberExtensions
{
	/// <summary>
	/// Formats the given byte count to a human-readable string.
	/// </summary>
	/// <param name="bytes"></param>
	public static string HumanSize (this int bytes)
		=> HumanSize ((float) bytes);

	/// <summary>
	/// Formats the given byte count to a human-readable string.
	/// </summary>
	/// <param name="bytes"></param>
	public static string HumanSize (this long bytes)
		=> HumanSize ((float) bytes);

	/// <summary>
	/// Formats the given byte count to a human-readable string.
	/// </summary>
	/// <param name="bytes"></param>
	public static string HumanSize (this float bytes)
		=> bytes switch
		{
			< Size.Kb => $"{bytes} byte(s)",
			< Size.Mb => $"{bytes / Size.Kb:F3} KB",
			< Size.Gb => $"{bytes / Size.Mb:F3} MB",
			_         => $"{bytes / Size.Gb:F3} GB"
		};

	/// <summary>
	/// Formats the given bits-per-second value to a human-readable string.
	/// </summary>
	/// <param name="bps"></param>
	public static string HumanSpeed (this int bps)
		=> HumanSpeed ((float) bps);

	/// <summary>
	/// Formats the given bits-per-second value to a human-readable string.
	/// </summary>
	/// <param name="bps"></param>
	public static string HumanSpeed (this long bps)
		=> HumanSpeed ((float) bps);

	/// <summary>
	/// Formats the given bits-per-second value to a human-readable string.
	/// </summary>
	/// <param name="bps"></param>
	public static string HumanSpeed (this float bps)
		=> bps switch
		{
			< Size.Kb => $"{bps:F2} bps",
			< Size.Mb => $"{bps / Size.Kb:F2} Kbps",
			< Size.Gb => $"{bps / Size.Mb:F2} Mbps",
			_         => $"{bps / Size.Gb:F2} Gbps"
		};

	/// <summary>
	/// Formats the given seconds value to a human-readable string.
	/// </summary>
	/// <param name="secs"></param>
	public static string HumanTime (this int secs)
		=> HumanTime ((float) secs);

	/// <summary>
	/// Formats the given seconds value to a human-readable string.
	/// </summary>
	/// <param name="secs"></param>
	public static string HumanTime (this long secs)
		=> HumanTime ((float) secs);

	/// <summary>
	/// Formats the given seconds value to a human-readable string.
	/// </summary>
	/// <param name="secs"></param>
	public static string HumanTime (this float secs)
		=> secs < 1
			? $"{secs * 1000} ms"
			: $"{secs:F3} sec";
}

/// <summary>
/// Defines some handy size constants.
/// </summary>
public static class Size
{
	/// <summary>
	/// One kilobyte (OK, a kibibyte to be perfectly correct)
	/// </summary>
	public const int Kb = 1024;

	/// <summary>
	/// One megabyte (OK, a mibibyte to be perfectly correct)
	/// </summary>
	public const int Mb = Kb * 1024;

	/// <summary>
	/// One gigabyte (OK, a gibibyte to be perfectly correct)
	/// </summary>
	public const int Gb = Mb * 1024;
}
