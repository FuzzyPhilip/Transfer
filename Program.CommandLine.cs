using System.CommandLine;


namespace Beinggs.Transfer;


// command line definitions: Commands, Arguments and Options
partial class Program
{
	#region Helpers

	enum Verbosity
	{
		Quiet,
		Minimal,
		Normal,
		Detailed,
		Diagnostic
	}

	#endregion Helpers

	#region Fields

	// constants
	public const string DefaultFileName = "transfer.dat";
	public const string FileNameHeader = "filename";
	public const string Anyone = "anyone";

	public const int MinTestSize = 1; // MB
	public const int MaxTestSize = 1024; // MB
	public const int DefTestSize = 10; // MB

	// globals
	public static LogLevel LogLevel;
	public static int Timeout;
	public static bool Measured;
	public static int Port;

	#endregion Fields

	#region Global options

	static readonly Option<Verbosity?> verbosity = new (
		aliases: new [] { "--verbosity", "-v", "/v" },
		description: "Level of detail in output messages",
		getDefaultValue: () => Verbosity.Quiet);

	static readonly Option<int> timeout = new (
		aliases: new[] { "--timeout", "-t", "/t" },
		description: "Timeout in seconds for any send or receive operation, or 0 for no timeout",
		getDefaultValue: () => 30);

	static readonly Option<bool> measured = new (
		aliases: new[] { "--measured", "-m", "/m" },
		description: "Set false to not show timing and performance data",
		getDefaultValue: () => true);

	static readonly Option<int> port = new (
		aliases: new [] { "--port", "-p", "/p" },
		description: "Specifies the port to listen or send on",
		getDefaultValue: () => 8888);

	#endregion Global options

	#region Send stuff

	static readonly Option<bool> repeat = new (
		aliases: new[] { "--repeat", "-r", "/r" },
		description: "Set true to repeat send operation forever",
		getDefaultValue: () => false);

	static readonly Argument<FileInfo?> file = new (
		name: "file",
		description: "The name of the file to send");

	static readonly Option<bool> includeFileName = new (
		aliases: new[] { "--include-filename", "-f", "/f" },
		description: $"Set false to send raw data with no \"{FileNameHeader}:name\\n\" (as UTF8 bytes) header",
		getDefaultValue: () => true);

	static readonly Argument<string> recipient = new (
			name: "recipient",
			description: $"Recipient machine, IP address, or \"{Anyone}\" to allow any remote client to connect");

	static readonly Option<int> testSize = new (
		aliases: new[] { "--size", "-s", "/s" },
		description: $"Test data size, in MB (between {MinTestSize} and {MaxTestSize})",
		getDefaultValue: () => DefTestSize);

	static readonly Command toCommand = new (
		name: "to",
		description: "Send a file or test data to a recipient")
	{
		recipient
	};

	#endregion Send stuff

	#region Receive stuff

	static readonly Argument<string?> fileName = new (
		name: "fileName",
		description: "The name of the file to receive",
		getDefaultValue: () => DefaultFileName);

	static readonly Option<int> maxSize = new (
		name: "--max-size",
		description: "Sets the maximum amount of data to receive, in MB, " +
				"or omit (or set to zero) to receive all sent data",
		getDefaultValue: () => 0);

	static readonly Argument<string> sender = new (
		name: "sender",
		description: "Sender machine or IP address");

	static readonly Command fromCommand = new (
		name: "from",
		description: "The sender of the file or test data")
	{
		sender
	};

	#endregion Receive stuff
}
