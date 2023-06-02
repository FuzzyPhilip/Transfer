namespace Beinggs.Transfer.Logging;


/// <summary>
/// Provides logging functionality.
/// </summary>
public static class Logger
{
	/// <summary>
	/// Indicates the logging level being used, as defined by <see cref="Logging.LogLevel"/>.
	/// </summary>
	public static LogLevel LogLevel { get; set; }

	/// <summary>
	/// Sets the <see cref="LogLevel"/> based on the given <see cref="Verbosity"/>.
	/// </summary>
	/// <remarks>
	/// <ul><li>
	/// Verbosity values <see cref="Verbosity.Quiet"/> and <see cref="Verbosity.Minimal"/> are mapped to
	/// <see cref="LogLevel.Quiet"/>
	/// </li><li>
	/// Verbosity value <see cref="Verbosity.Normal"/> is mapped to <see cref="LogLevel.Info"/>
	/// </li><li>
	/// Verbosity values <see cref="Verbosity.Detailed"/> and <see cref="Verbosity.Diagnostic"/> are mapped to
	/// <see cref="LogLevel.Verbose"/>
	/// </li><li>
	/// All other verbosity values are mapped to <see cref="LogLevel.Info"/>.
	/// </li></ul>
	/// </remarks>
	/// <param name="verbosity">The 'standard' command line verbosity value.</param>
	public static void SetLogLevel (Verbosity? verbosity)
		=> LogLevel = verbosity switch
		{
			Verbosity.Quiet or Verbosity.Minimal => LogLevel.Quiet,
			Verbosity.Normal => LogLevel.Info,
			Verbosity.Detailed or Verbosity.Diagnostic or null => LogLevel.Verbose,
			_ => LogLevel.Info
		};

	/// <summary>
	/// Logs this string using <see cref="LogLevel.Info"/>.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="lineEnding">
	/// An optional line ending string. If not provided, <see cref="Environment.NewLine"/> is used.
	/// </param>
	public static void Log (this string message, string? lineEnding = null)
		=> Log (message, LogLevel.Info, lineEnding);

	/// <summary>
	/// Logs this string using the given log level.
	/// </summary>
	/// <remarks>
	/// As <see cref="LogLevel.Error"/> is the lowest log level, errors will always be logged.
	/// <para>
	/// A message will be logged if the specified <paramref name="level"/> is less than or equal to the
	/// <see cref="Logger.LogLevel"/> set at start time. For example, if <see cref="Logger.LogLevel"/> is set to
	/// <see cref="LogLevel.Quiet"/>, messages logged using <see cref="LogLevel.Info"/> or <see cref="LogLevel.Verbose"/>
	/// won't appear in the log output.
	/// </para>
	/// </remarks>
	/// <param name="message">The message to log.</param>
	/// <param name="level">The <see cref="LogLevel"/> to use.</param>
	/// <param name="lineEnding">An optional line ending string to use instead of <see cref="Environment.NewLine"/>.</param>
	public static void Log (this string message, LogLevel level, string? lineEnding = null)
	{
		if (level == LogLevel.Error)
		{
			Console.Error.Write (getLine ($"Error: {message}"));
		}
		else if (level <= LogLevel)
		{
			if (level == LogLevel.Warning)
				Console.Write (getLine ($"Warning: {message}"));
			else
				Console.Write (getLine (message));
		}

		string getLine (string msg)
			=> $"{msg}{(lineEnding is null ? Environment.NewLine : lineEnding)}";
	}
}
