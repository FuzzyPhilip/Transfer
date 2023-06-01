using System.Net;
using System.Net.Sockets;

using Beinggs.Transfer.Extensions;
using Beinggs.Transfer.Logging;


namespace Beinggs.Transfer;


/// <summary>
/// Sends either a file or some test data to a remote sender.
/// </summary>
public class Sender
{
	readonly bool _repeat;

	FileInfo? _file;
	bool _includeFileName;
	int _testSize;

	/// <summary>
	/// Creates a new instance of <see cref="Sender"/>.
	/// </summary>
	/// <param name="repeat"></param>
	public Sender (bool repeat)
		=> _repeat = repeat;

	/// <summary>
	/// Sends a file to <paramref name="recipient"/>.
	/// </summary>
	/// <param name="file">The file to read from.</param>
	/// <param name="includeFileName">Set to <see langword="true"/> to include the file's name in a header.</param>
	/// <param name="recipient">The remote recipient's IP address or name.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Thrown when an invalid parameter value is given.</exception>
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

	/// <summary>
	/// Sends test data from <paramref name="recipient"/>.
	/// </summary>
	/// <param name="testSize">The size, in KB, of the test data to send.</param>
	/// <param name="recipient">The recipient to send the test data to.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
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
			$"\nConnected to {address}:{Program.Port}; sending data...".Log (LogLevel.Quiet);

			using var output = client.GetStream();
			using var input = await GetInputStream (output);

			await input.CopyToAsync (output);

			$"Send complete.".Log (LogLevel.Quiet);
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
				await output.WriteAsync ($"{Program.FileNameHeader}:{_file.Name}\n".ToUtf8Bytes());

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
