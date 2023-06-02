namespace Beinggs.Transfer.Logging;


/// <summary>
/// Defines the values for the logging level.
/// </summary>
public enum LogLevel
{
	/// <summary>
	/// Specifies that only error message are to be logged.
	/// </summary>
	Error,

	/// <summary>
	/// Specifies that only warning and error messages are to be logged.
	/// </summary>
	Warning,

	/// <summary>
	/// Specifies that minimal detail, warning and error messages are to be logged.
	/// </summary>
	Quiet,

	/// <summary>
	/// Specifies that informative, warning and error messages are to be logged.
	/// </summary>
	Info,

	/// <summary>
	/// Specifies that all messages are to be logged.
	/// </summary>
	Verbose
}
