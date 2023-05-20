namespace Beinggs.Transfer;


// command handlers and helpers
partial class Program
{
	#region Send

	static void ToCommand (int timeout, bool measured, int port, bool repeat,
			FileInfo? file, bool includeFileName, int testSize, string recipient)
	{
		if (file is not null)
			SendFile (timeout, measured, port, repeat, file, includeFileName, recipient);
		else
			SendTest (timeout, measured, port, repeat, testSize, recipient);
	}

	static void SendFile (int timeout, bool measured, int port, bool repeat,
			FileInfo file, bool includeFileName, string recipient)
	{
		Console.WriteLine ($"Sending {file.Name} " +
				$"to {recipient} {SendInfo (timeout, measured, port, repeat, includeFileName)}:");

		File.ReadLines (file.FullName).ToList()
			.ForEach (Console.WriteLine);
	}

	static void SendTest (int timeout, bool measured, int port, bool repeat,
			int testSize, string recipient)
		=> Console.WriteLine ($"Sending {testSize} MB " +
				$"to {recipient} {SendInfo (timeout, measured, port, repeat)}");

	#endregion Send

	#region Receive

	static void FromCommand (int timeout, bool measured, int port,
			string? fileName, int? maxSize, string sender)
	{
		if (fileName is not null)
			ReceiveFile (timeout, measured, port, fileName, sender);
		else
			ReceiveTest (timeout, measured, port, maxSize, sender);
	}

	static void ReceiveFile (int timeout, bool measured, int port, string fileName, string sender)
		=> Console.WriteLine ($"Receiving file {fileName} " +
				$"from {sender} {ReceiveInfo (timeout, measured, port)}");

	static void ReceiveTest (int timeout, bool measured, int port, int? maxSize, string sender)
		=> Console.WriteLine ($"Receiving {(maxSize > 0 ? $"up to {maxSize} MB of " : "")}test data " +
				$"from {sender} {ReceiveInfo (timeout, measured, port)}");

	#endregion Receive

	#region Helpers

	static string SendInfo (int timeout, bool measured, int port, bool repeat, bool? includeFileName = null)
		=> "(" +
			$"timeout: {timeout}" +
			$"; {(measured ? "" : "not ") + "measured"}" +
			$"; port: {port}" +
			$"; {(repeat ? "" : "not ") + "repeating"}" +
			(includeFileName is null ? "" :
					$"; {(includeFileName.Value ? "" : "not ") + "including file name"}") +
			")";

	static string ReceiveInfo (int timeout, bool measured, int port)
		=> "(" +
			$"timeout: {timeout}" +
			$"; {(measured ? "" : "not ") + "measured"}" +
			$"; port: {port}" +
			")";

	#endregion Helpers
}
