using System.Buffers;
using System.Diagnostics;


namespace Transfer.Extensions;


static class StreamExtensions
{
	public static async Task<( int milliseconds, int bytes )> CopyToWithTimingAsync (this Stream source,
			Stream destination, int bufferSize = 65536, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull (destination);

		if (bufferSize < 1)
			throw new ArgumentOutOfRangeException (nameof (bufferSize), bufferSize,
					"buffer size must be positive");

		if (!destination.CanWrite)
			throw new InvalidOperationException ("destination stream is not writeable");

		var buffer = ArrayPool<byte>.Shared.Rent (bufferSize);
		var timer = Stopwatch.StartNew();
		var totalBytes = 0;

		timer.Start();

		try
		{
			int bytesRead;

			while ((bytesRead = await source.ReadAsync (new Memory<byte> (buffer), cancellationToken)) != 0)
			{
				totalBytes += bytesRead;

				await destination.WriteAsync (new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken);
			}
		}
		finally
		{
			ArrayPool<byte>.Shared.Return (buffer);
		}

		timer.Stop();

		return ( (int) timer.ElapsedMilliseconds, totalBytes );
	}
}
