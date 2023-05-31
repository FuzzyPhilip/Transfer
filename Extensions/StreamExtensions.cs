using System.Buffers;
using System.Diagnostics;


namespace Beinggs.Transfer.Extensions;


/// <summary>
/// Handy-dandy <see cref="Stream"/> extension methods.
/// </summary>
static class StreamExtensions
{
	/// <summary>
	/// Copies this stream to the <paramref name="destination"/> stream with the given parameters.
	/// </summary>
	/// <param name="source">The stream to copy from.</param>
	/// <param name="destination">The stream to copy to.</param>
	/// <param name="maxBytes">An optional maximum size, in bytes, to copy (0 is unlimited).</param>
	/// <param name="maxSecs">
	/// An optional maximum time, in seconds, to wait for the copy to complete (0 is unlimited).
	/// </param>
	/// <param name="bufferSize">An optional buffer size, in bytes, to use.</param>
	/// <param name="cancellationToken">An optional cancellation token.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid buffer size is given.</exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown of the <paramref name="destination"/> is not writeable.
	/// </exception>
	public static async Task<(int milliseconds, int bytes)> CopyToWithTimingAsync (this Stream source,
			Stream destination, int maxBytes = 0, int maxSecs = 0,
			int bufferSize = 65536, CancellationToken cancellationToken = default)
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
		var maxMS = maxSecs * 1000;

		timer.Start();

		try
		{
			var bytesRead = 0;

			do
			{
				var bytesToRead = maxBytes == 0
					? buffer.Length
					: Math.Min (maxBytes - bytesRead, buffer.Length);

				bytesRead = await source.ReadAsync (new Memory<byte> (buffer, 0, bytesToRead), cancellationToken);

				if (bytesRead > 0)
				{
					totalBytes += bytesRead;

					await destination.WriteAsync (new ReadOnlyMemory<byte> (buffer, 0, bytesRead), cancellationToken);
				}
			} while (bytesRead > 0 &&
					(maxMS == 0 || timer.ElapsedMilliseconds < maxMS) &&
					(maxBytes == 0 || totalBytes < maxBytes));
		}
		finally
		{
			ArrayPool<byte>.Shared.Return (buffer);
		}

		timer.Stop();

		return ((int) timer.ElapsedMilliseconds, totalBytes);
	}
}
