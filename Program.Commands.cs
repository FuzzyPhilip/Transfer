using Beinggs.Transfer.Extensions;


namespace Beinggs.Transfer;


// command handlers and helpers
partial class Program
{
	#region Send

	static async Task ToCommand (Verbosity? verbosity, int timeout, bool measured, int port,
			bool repeat, FileInfo? file, bool includeFileName, int testSize, string recipient)
	{
		// can't bind globals to root command, so have to handle them in _every_ command :-/
		SetGlobals (verbosity, timeout, measured, port);

		if (file is not null)
			await SendFile (repeat, file, includeFileName, recipient);
		else
			await SendTest (repeat, testSize, recipient);
	}

	static async Task SendFile (bool repeat, FileInfo file, bool includeFileName, string recipient)
	{
		$"Sending file {file.Name} to {recipient} {SendFileInfo (repeat, includeFileName)}:".Log();

		await new Sender (repeat).SendFileAsync (file, includeFileName, recipient);
	}

	static async Task SendTest (bool repeat, int testSize, string recipient)
	{
		$"Sending test of {testSize} MB to {recipient} {SendTestInfo (repeat)}".Log();

		await new Sender (repeat).SendTestAsync (testSize, recipient);
	}

	#endregion Send

	#region Receive

	static async Task FromCommand (Verbosity? verbosity, int timeout, bool measured, int port,
			string? fileName, int maxSize, int maxTime, string sender)
	{
		// can't bind globals to root command, so have to handle them in _every_ command :-/
		SetGlobals (verbosity, timeout, measured, port);

		if (fileName is not null)
			await ReceiveFile (new FileInfo (fileName), sender);
		else
			await ReceiveTest (maxSize, maxTime, sender);
	}

	static async Task ReceiveFile (FileInfo file, string sender)
	{
		$"Receiving file {file.Name} from {sender} {ReceiveInfo()}...".Log();

		await new Receiver().ReceiveFileAsync (file, sender);
	}

	static async Task ReceiveTest (int maxSize, int maxTime, string sender)
	{
		$"Receiving {(maxSize > 0 ? $"up to {maxSize} MB of " : "")}test data from {sender} {ReceiveInfo()}...".Log();

		await new Receiver().ReceiveTestAsync (maxSize, maxTime, sender);
	}

	#endregion Receive

	#region Helpers

	static void SetGlobals (Verbosity? verbosity, int timeout, bool measured, int port)
	{
		SetLogLevel (verbosity);
		Timeout = timeout;
		Measured = measured;
		Port = port;
	}

	static void SetLogLevel (Verbosity? verbosity)
		=> LogLevel = verbosity switch
		{
			Verbosity.Quiet or Verbosity.Minimal => LogLevel.Quiet,
			Verbosity.Normal => LogLevel.Info,
			Verbosity.Detailed or Verbosity.Diagnostic => LogLevel.Verbose,
			_ => LogLevel.Info
		};

	static string SendFileInfo (bool repeat, bool includeFileName)
		=> "(" +
			$"log level: {LogLevel}" +
			$"; timeout: {Timeout}" +
			$"; {(Measured ? "" : "not ") + "measured"}" +
			$"; port: {Port}" +
			$"; {(repeat ? "" : "not ") + "repeating"}" +
			$"; {(includeFileName ? "" : "not ") + "including file name"}" +
			")";

	static string SendTestInfo (bool repeat)
		=> "(" +
			$"log level: {LogLevel}" +
			$"; timeout: {Timeout}" +
			$"; {(Measured ? "" : "not ") + "measured"}" +
			$"; port: {Port}" +
			$"; {(repeat ? "" : "not ") + "repeating"}" +
			")";

	static string ReceiveInfo()
		=> "(" +
			$"log level: {LogLevel}" +
			$"; timeout: {Timeout}" +
			$"; {(Measured ? "" : "not ") + "measured"}" +
			$"; port: {Port}" +
			")";

	#endregion Helpers
}
