using System.Text;


namespace Beinggs.Transfer.Extensions;


public static class StringExtensions
{
	/// <summary>
	/// Matches this string to <see langword="true"/> or <see langword="false"/> based on its truthiness or otherwise.
	/// </summary>
	/// <remarks>
	/// By default, a <see langword="null"/> or empty value is interpreted as <see langword="true"/> as the presence
	/// of a bool option with no argument normally indicates the user wants to enable or turn on that option.
	/// </remarks>
	/// <param name="s">This string.</param>
	/// <param name="emptyDefault">The result if <paramref name="s"/> is <see langword="null"/> or empty.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static bool ToBool (this string? s, bool emptyDefault = true)
		=> s switch
		{
			"TRUE" or "True" or "true" or "T" or "t"
				or "YES" or "Yes" or "yes" or "Y" or "y"
				or "1" => true,
			"FALSE" or "False" or "false" or "F" or "f"
				or "NO" or "No" or "no" or "N" or "n"
				or "0" => false,
			"" or null => emptyDefault,
			_ => throw new InvalidOperationException ($"Invalid boolean option '{s}'")
		};

	public static byte[] ToUtf8 (this string s)
		=> Encoding.UTF8.GetBytes (s);

	public static void Log (this string message)
		=> Log (message, LogLevel.Info);

	public static void Log (this string message, LogLevel level)
	{
		if (level == LogLevel.Error)
		{
			Console.Error.WriteLine ($"Error: {message}");
		}
		else if (level <= Program.LogLevel)
		{
			if (level == LogLevel.Warning)
				Console.WriteLine ($"Warning: {message}");
			else
				Console.WriteLine (message);
		}
	}
}
