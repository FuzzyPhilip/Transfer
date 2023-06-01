using Beinggs.Transfer.Extensions;
using Beinggs.Transfer.Logging;


namespace Beinggs.Transfer;


// command handlers and helpers
partial class Program
{
	#region Send

	static async Task ToCommand (Verbosity? verbosity, string? measured, int port,
			bool repeat, FileInfo? file, bool includeFileName, int testSize, string recipient)
	{
		// can't bind globals to root command, so have to handle them in _every_ command :-/
		SetGlobals (verbosity, measured.ToBool(), port);

		if (file is not null)
			await SendFile (repeat, file, includeFileName, recipient);
		else
			await SendTest (repeat, testSize, recipient);
	}

	static async Task SendFile (bool repeat, FileInfo file, bool includeFileName, string recipient)
	{
		$"Sending file {file.Name} to {recipient} {SendFileInfo (repeat, includeFileName)}:".Log (LogLevel.Quiet);

		await new Sender (repeat).SendFileAsync (file, includeFileName, recipient);
	}

	static async Task SendTest (bool repeat, int testSize, string recipient)
	{
		$"Sending test of {testSize} MB to {recipient} {SendTestInfo (repeat)}".Log (LogLevel.Quiet);

		await new Sender (repeat).SendTestAsync (testSize, recipient);
	}

	#endregion Send

	#region Receive

	static async Task FromCommand (Verbosity? verbosity, string? measured, int port,
			string? fileName, int maxSize, int maxTime, string sender)
	{
		// can't bind globals to root command, so have to handle them in _every_ command :-/
		SetGlobals (verbosity, measured.ToBool(), port);

		if (fileName is not null)
			await ReceiveFile (new FileInfo (fileName), sender);
		else
			await ReceiveTest (maxSize, maxTime, sender);
	}

	static async Task ReceiveFile (FileInfo file, string sender)
	{
		$"Receiving file {file.Name} from {sender} {ReceiveInfo()}...".Log (LogLevel.Quiet);

		await new Receiver().ReceiveFileAsync (file, sender);
	}

	static async Task ReceiveTest (int maxSize, int maxTime, string sender)
	{
		($"Receiving {(maxSize > 0 ? $"up to {maxSize} MB of " : "")}test data " +
			$"{(maxTime > 0 ? $"for up to {maxTime} seconds " : "")}" +
			$"from {sender} {ReceiveInfo()}...").Log (LogLevel.Quiet);

		await new Receiver().ReceiveTestAsync (maxSize, maxTime, sender);
	}

	#endregion Receive

	#region Helpers

	static void SetGlobals (Verbosity? verbosity, bool measured, int port)
	{
		Logger.SetLogLevel (verbosity);

		Measured = measured;
		Port = port;
	}

	static string SendFileInfo (bool repeat, bool includeFileName)
		=> "(" +
			$"log level: {Logger.LogLevel}" +
			$"; {(Measured ? "" : "not ") + "measured"}" +
			$"; port: {Port}" +
			$"; {(repeat ? "" : "not ") + "repeating"}" +
			$"; {(includeFileName ? "" : "not ") + "including file name"}" +
			")";

	static string SendTestInfo (bool repeat)
		=> "(" +
			$"log level: {Logger.LogLevel}" +
			$"; {(Measured ? "" : "not ") + "measured"}" +
			$"; port: {Port}" +
			$"; {(repeat ? "" : "not ") + "repeating"}" +
			")";

	static string ReceiveInfo()
		=> "(" +
			$"log level: {Logger.LogLevel}" +
			$"; {(Measured ? "" : "not ") + "measured"}" +
			$"; port: {Port}" +
			")";

	#endregion Helpers
}
