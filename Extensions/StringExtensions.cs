using System.Text;


namespace Beinggs.Transfer.Extensions;


/// <summary>
/// Handy-dandy <see cref="string"/> extension methods.
/// </summary>
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
	/// <returns>
	/// <see langword="true"/> if <paramref name="s"/> smells truthy (TRUE, True, true, T, t, YES, Yes, yes, Y, y, or 1),
	/// or <see langword="false"/> if <paramref name="s"/> smells falsy (the opposite to the truthy values).
	/// <para>
	/// If <paramref name="s"/> is <see langword="null"/> or empty, the <paramref name="emptyDefault"/> value is returned.
	/// </para>
	/// </returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static bool ToBool (this string? s, bool emptyDefault = true)
		=> s?.Trim() switch
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

	/// <summary>
	/// Converts this string to a UTF8 byte array.
	/// </summary>
	/// <param name="s">The string to convert.</param>
	public static byte[] ToUtf8 (this string s)
		=> Encoding.UTF8.GetBytes (s);

	/// <summary>
	/// Logs this string using <see cref="LogLevel.Info"/>.
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void Log (this string message)
		=> Log (message, LogLevel.Info);

	/// <summary>
	/// Logs this string using the given log level.
	/// </summary>
	/// <remarks>
	/// As <see cref="LogLevel.Error"/> is the lowest log level, errors will always be logged.
	/// <para>
	/// 
	/// </para>
	/// </remarks>
	/// <param name="message">The message to log.</param>
	/// <param name="level">The <see cref="LogLevel"/> to use.</param>
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
