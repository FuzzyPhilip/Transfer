using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;

using Beinggs.Transfer.Extensions;

using Transfer.Extensions;

namespace Beinggs.Transfer;


class Receiver
{
	FileInfo _file = default!;
	int _maxSize;

	public Task ReceiveFileAsync (FileInfo file, string sender)
	{
		if (file is null)
			throw new InvalidOperationException ("A file name must be specified");

		if (string.IsNullOrWhiteSpace (sender))
			throw new InvalidOperationException ("A sender must be specified");

		_file = file;

		return Receive (sender);
	}

	public Task ReceiveTestAsync (int maxSize, string sender)
	{
		if (maxSize is < 0)
			throw new InvalidOperationException ("A zero or positive maximum size must be specified");

		_maxSize = maxSize;

		return Receive (sender);
	}

	async Task Receive (string sender)
	{
		const float kb = 1024f;
		const float mb = kb * 1024f;

		try
		{
			using TcpClient client = new (sender, Program.Port);

			using var input = client.GetStream();
			using var output = await GetOutputStream (input);

			// copy the stream to output and time it
			var (milliseconds, bytes) = await input.CopyToWithTimingAsync (output);

			var bits = bytes * 8f;
			var secs = milliseconds / 1000f;

			var size = (float) bytes switch
			{
				< kb => $"{bytes} byte(s)",
				< mb => $"{bytes / kb:F3} KB",
				_ => $"{bytes / mb:F3} MB"
			};

			var time = secs < 1
				? $"{milliseconds} ms"
				: $"{secs:F3} sec";

			var speed = bytes < 10 * kb
				? $"{bits / kb / secs:F2} Kbps"
				: $"{bits / mb / secs:F2} Mbps";

			var receivedMsg = $"Successfully received {size} in {time} @ {speed}";

			if (_file is not null)
				receivedMsg += $" into {_file.Name}";

			receivedMsg.Log();
		}
		catch (Exception ex)
		{
			$"Failed to receive data due to: {ex.Message}".Log (LogLevel.Error);
		}
	}

	async Task<Stream> GetOutputStream (Stream input)
	{
		if (_file is not null)
		{
			Stream output;

			if (GetFileName (input, $"{Program.FileNameHeader}:",
					out var fileName, out var lineRead))
			{
				// if no filename given on cmd line, use the one from the header
				if (_file.Name == Program.DefaultFileName)
				{
#if DEBUG
					fileName = "TEST." + fileName;
#endif
					// we got a filename, so create that file instead of the default
					_file = new FileInfo (fileName);
				}

				output = _file.OpenWrite();
			}
			else
			{
				// no filename, so create given _file and push line already read to output
				output = _file.OpenWrite();

				if (lineRead is not null)
					await output.WriteAsync (lineRead.ToUtf8());
			}

			return output;
		}
		else
		{
			return new DiscardStream();
		}
	}

	static bool GetFileName (Stream input, string prefix,
			[NotNullWhen (true)] out string? fileName, out string? lineRead)
	{
		using StreamReader reader = new (input, Encoding.UTF8, leaveOpen: true);

		lineRead = reader.ReadLine();

		if (lineRead?.StartsWith (prefix) == true)
		{
			fileName = lineRead [prefix.Length..];

			return true;
		}
		
		fileName = null;

		return false;
	}
}
