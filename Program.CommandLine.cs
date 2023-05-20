using System.CommandLine;


namespace Beinggs.Transfer;


// command line definitions: Commands, Arguments and Options
partial class Program
{
	#region Constants

	const string ReceivedFileName = "transfer.dat";
	const string FileNameHeader = "filename:";

	#endregion Constants

	#region Global options

	static readonly Option<int> timeout = new (
		aliases: new[] { "--timeout", "-t", "/t" },
		description: "Timeout in seconds for any send or receive operation, or 0 for no timeout",
		getDefaultValue: () => 30);

	static readonly Option<bool> measured = new (
		aliases: new[] { "--measured", "-m", "/m" },
		description: "Set false to not show timing and performance data",
		getDefaultValue: () => true);

	#endregion Global options

	#region Send stuff

	static readonly Option<bool> repeat = new (
		aliases: new[] { "--repeat", "-r", "/r" },
		description: "Set true to repeat send operation forever",
		getDefaultValue: () => false);

	static readonly Argument<FileInfo?> file = new (
		name: "file",
		description: "The file to send");

	static readonly Option<bool> includeFileName = new (
		aliases: new[] { "--include-filename", "-f", "/f" },
		description: $"Set false to send raw data with no \"{FileNameHeader}:name\\0\" (as UTF8 bytes) header",
		getDefaultValue: () => true);

	static readonly Argument<string> recipient = new (
			name: "recipient",
			description: "Recipient machine, IP address, or 'anyone' to allow any remote client to connect");

	static readonly Option<int> testSize = new (
		aliases: new[] { "--size", "-s", "/s" },
		description: "Test data size, in MB",
		getDefaultValue: () => 10);

	static readonly Command toCommand = new (
		name: "to",
		description: "Send a file or test data to a recipient")
	{
		recipient
	};

	#endregion Send stuff

	#region Receive stuff

	static readonly Argument<string?> fileName = new (
		name: "filename",
		description: $"The name of the file in which to save the received file data",
		getDefaultValue: () => ReceivedFileName);

	static readonly Option<int?> maxSize = new (
		name: "--max-size",
		description: "Sets the maximum amount of data to receive, in MB");

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
