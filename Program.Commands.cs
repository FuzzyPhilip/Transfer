namespace Beinggs.Transfer;


// command handlers and helpers
partial class Program
{
	#region Send

	static void ToCommand (int timeout, bool measured, bool repeat,
			FileInfo? file, bool includeFileName, int testSize, string recipient)
	{
		if (file is not null)
			SendFile (timeout, measured, repeat, file, includeFileName, recipient);
		else
			SendTest (timeout, measured, repeat, testSize, recipient);
	}

	static void SendFile (int timeout, bool measured, bool repeat, FileInfo file, bool includeFileName, string recipient)
	{
		Console.WriteLine ($"Sending {file.Name} " +
				$"to {recipient} {SendInfo (timeout, measured, repeat, includeFileName)}:");

		File.ReadLines (file.FullName).ToList()
			.ForEach (Console.WriteLine);
	}

	static void SendTest (int timeout, bool measured, bool repeat, int testSize, string recipient)
		=> Console.WriteLine ($"Sending {testSize} MB " +
				$"to {recipient} {SendInfo (timeout, repeat, measured)}");

	#endregion Send

	#region Receive

	static void FromCommand (int timeout, bool measured, string? fileName, int? maxSize, string sender)
	{
		if (fileName is not null)
			ReceiveFile (timeout, measured, fileName, sender);
		else
			ReceiveTest (timeout, measured, maxSize, sender);
	}

	static void ReceiveFile (int timeout, bool measured, string fileName, string sender)
		=> Console.WriteLine ($"Receiving file {fileName} " +
				$"from {sender} {ReceiveInfo (timeout, measured)}");

	static void ReceiveTest (int timeout, bool measured, int? maxSize, string sender)
		=> Console.WriteLine ($"Receiving {(maxSize > 0 ? $"up to {maxSize} MB of " : "")}test data " +
				$"from {sender} {ReceiveInfo (timeout, measured)}");

	#endregion Receive

	#region Helpers

	static string SendInfo (int timeout, bool measured, bool repeat, bool? includeFileName = null)
		=> "(" +
			$"timeout: {timeout}" +
			$"; {(measured ? "" : "not ") + "measured"}" +
			$"; {(repeat ? "" : "not ") + "repeating"}" +
			(includeFileName is null ? "" :
					$"; {(includeFileName.Value ? "" : "not ") + "including file name"}") +
			")";

	static string ReceiveInfo (int timeout, bool measured)
		=> "(" +
			$"timeout: {timeout}" +
			$"; {(measured ? "" : "not ") + "measured"}" +
			")";

	#endregion Helpers
}
