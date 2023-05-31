using System.Net;
using System.Net.Sockets;


using Beinggs.Transfer.Extensions;


namespace Beinggs.Transfer;


class Sender
{
	readonly bool _repeat;

	FileInfo? _file;
	bool _includeFileName;
	int _testSize;

	public Sender (bool repeat)
		=> _repeat = repeat;

	public Task SendFileAsync (FileInfo file, bool includeFileName, string recipient)
	{
		if (file is null)
			throw new InvalidOperationException ("A file name must be specified");

		if (string.IsNullOrWhiteSpace (recipient))
			throw new InvalidOperationException ("A recipient must be specified");

		_file = file;
		_includeFileName = includeFileName;

		return Send (recipient);
	}

	public Task SendTestAsync (int testSize, string recipient)
	{
		if (testSize is < 1 or > Program.MaxTestSize)
		{
			throw new InvalidOperationException (
					$"Test size must be between {Program.MinTestSize} and {Program.MaxTestSize} (in MB)");
		}

		_testSize = testSize;

		return Send (recipient);
	}

	async Task Send (string recipient)
	{
		try
		{
			TcpListener server = new (IPAddress.Any, Program.Port);
			server.Start();

			"Server started; Listening...".Log (LogLevel.Quiet);

			do
			{
				using var client = await server.AcceptTcpClientAsync();
				var address = client.GetClientAddress();

				// if it's not who we're expecting, skip it
				if (recipient != Program.Anyone && address != recipient)
					$"\nConnection attempted from invalid client ({address}); dropped".Log (LogLevel.Warning);
				else // hand it off and wait for next, if repeating
					await SendData (client, address);
			}
			while (_repeat);
		}
		catch (Exception ex)
		{
			$"Failed to send data due to: {ex.Message}".Log (LogLevel.Error);
		}
	}

	async Task SendData (TcpClient client, string address)
	{
		try
		{
			$"\nConnected to {address}:{Program.Port}; sending data...".Log (LogLevel.Info);

			using var output = client.GetStream();
			using var input = await GetInputStream (output);

			await input.CopyToAsync (output);

			$"Successfully sent data".Log (LogLevel.Quiet);
		}
		catch (Exception ex)
		{
			$"Failed to send data due to: {ex.Message}".Log (LogLevel.Error);
		}
	}

	async Task<Stream> GetInputStream (Stream output)
	{
		if (_file is not null)
		{
			// send file data
			if (_includeFileName)
				await output.WriteAsync ($"{Program.FileNameHeader}:{_file.Name}\n".ToUtf8());

			return _file.OpenRead();
		}
		else if (_testSize > 0)
		{
			byte[] testData;
			var rnd = new Random ((int) DateTime.Now.Ticks);
			var dataSize = _testSize * 1024 * 1024;

			testData = new byte [dataSize];

			for (var i = 0; i < dataSize; i++)
				testData [i] = (byte) rnd.Next (33, 127);

			return new MemoryStream (testData);
		}

		throw new InvalidOperationException ("Either a file or test data size must be specified");
	}
}
