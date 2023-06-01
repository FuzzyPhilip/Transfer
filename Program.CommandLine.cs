using System.CommandLine;

using Beinggs.Transfer.Logging;


namespace Beinggs.Transfer;


// command line definitions: Commands, Arguments and Options
partial class Program
{
	#region Fields

	/// <summary>
	/// Defines the default received ffile name (if the <c>includeFileName</c> option isn't specified).
	/// </summary>
	public const string DefaultReceiveFileName = "transfer.dat";

	/// <summary>
	/// Defines the header key for the filename header, if provided.
	/// </summary>
	public const string FileNameHeader = "filename";

	/// <summary>
	/// Defines the 'anyone' string to specify sending/receiving to/from anyone.
	/// </summary>
	public const string Anyone = "anyone";

	/// <summary>
	/// Defines the minimum test size option value, in MB.
	/// </summary>
	public const int MinTestSize = 1; // MB

	/// <summary>
	/// Defines the maximum test size option value, in MB.
	/// </summary>
	public const int MaxTestSize = 1024; // MB

	/// <summary>
	/// Defines the default test size option value, in MB.
	/// </summary>
	public const int DefTestSize = 10; // MB

	/// <summary>
	/// Indicates whether performance should be measured when receiving test data or a file.
	/// </summary>
	public static bool Measured { get; set; }

	/// <summary>
	/// Profides the port value to connect to or listen on.
	/// </summary>
	public static int Port { get; set; }

	#endregion Fields

	#region Global options

	static readonly Option<Verbosity?> globalOptVerbosity = new (
		aliases: new [] { "--verbosity", "-v", "/v" },
		description: "Level of detail in output messages",
		getDefaultValue: () => Verbosity.Normal);

	static readonly Option<string?> globalOptMeasured = new (
		aliases: new[] { "--measured", "-m", "/m" },
		description: "Set false to not show timing and performance data",
		getDefaultValue: () => "true");

	static readonly Option<int> globalOptPort = new (
		aliases: new [] { "--port", "-p", "/p" },
		description: "Specifies the port to listen or send on",
		getDefaultValue: () => 8888);

	#endregion Global options

	#region Send stuff

	static readonly Option<bool> optRepeat = new (
		aliases: new[] { "--repeat", "-r", "/r" },
		description: "Set true to repeat send operation forever",
		getDefaultValue: () => false);

	static readonly Argument<FileInfo?> argFile = new (
		name: "file",
		description: "The name of the file to send");

	static readonly Option<bool> optIncludeFileName = new (
		aliases: new[] { "--include-filename", "-f", "/f" },
		description: $"Set false to send raw data with no \"{FileNameHeader}:name\\n\" (as UTF8 bytes) header",
		getDefaultValue: () => true);

	static readonly Argument<string> argRecipient = new (
			name: "recipient",
			description: $"Recipient machine, IP address, or \"{Anyone}\" to allow any remote client to connect");

	static readonly Option<int> optTestSize = new (
		aliases: new[] { "--size", "-s", "/s" },
		description: $"Test data size, in MB (between {MinTestSize} and {MaxTestSize})",
		getDefaultValue: () => DefTestSize);

	static readonly Command cmdTo = new (
		name: "to",
		description: "Send a file or test data to a recipient")
	{
		argRecipient
	};

	#endregion Send stuff

	#region Receive stuff

	static readonly Argument<string?> argFileName = new (
		name: "fileName",
		description: "The name of the file to receive",
		getDefaultValue: () => DefaultReceiveFileName);

	static readonly Option<int> optMaxSize = new (
		name: "--max-size",
		description: "Sets the maximum amount of data to receive, in MB, " +
				"or omit (or set to zero) to receive all sent data",
		getDefaultValue: () => 0);

	static readonly Option<int> optMaxTime = new (
		name: "--max-time",
		description: "Sets the maximum amount of time to receive data for, in seconds, " +
				"or omit (or set to zero) to receive all sent data",
		getDefaultValue: () => 0);

	static readonly Argument<string> argSender = new (
		name: "sender",
		description: "Sender machine or IP address");

	static readonly Command cmdFrom = new (
		name: "from",
		description: "The sender of the file or test data")
	{
		argSender
	};

	#endregion Receive stuff
}
