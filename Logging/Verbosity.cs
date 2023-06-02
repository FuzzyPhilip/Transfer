namespace Beinggs.Transfer.Logging;


/// <summary>
/// Defines the values for the verbosity command-line option.
/// </summary>
public enum Verbosity
{
	/// <summary>
	/// Specifies that minimal detail, warning and error messages are to be logged.
	/// </summary>
	Quiet,
	
	/// <summary>
	/// Specifies that minimal detail, warning and error messages are to be logged.
	/// </summary>
	Minimal,

	/// <summary>
	/// Specifies that informative, warning and error messages are to be logged.
	/// </summary>
	Normal,

	/// <summary>
	/// Specifies that all messages are to be logged.
	/// </summary>
	Detailed,

	/// <summary>
	/// Specifies that all messages are to be logged.
	/// </summary>
	Diagnostic
}
