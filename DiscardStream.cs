namespace Beinggs.Transfer;


/// <summary>
/// Provides a 'null sink' <see cref="Stream"/> implementation which discards all data written to it.
/// </summary>
public class DiscardStream: Stream
{
	/// <inheritdoc/>
	public override bool CanRead => false;

	/// <inheritdoc/>
	public override bool CanSeek => false;

	/// <inheritdoc/>
	public override bool CanWrite => true;

	/// <inheritdoc/>
	public override long Length => 0;

	/// <inheritdoc/>
	public override long Position { get; set; }

	/// <summary>
	/// Performs no action in <see cref="DiscardStream"/>.
	/// </summary>
	public override void Flush() {}

	/// <summary>
	/// Unimplemented override which is supposed to read from the stream.
	/// </summary>
	/// <param name="buffer">The data buffer (ignored).</param>
	/// <param name="offset">The offset into the buffer to start writing from (ignored).</param>
	/// <param name="count">The number of bytes to write from the buffer (ignored).</param>
	/// <exception cref="NotSupportedException">
	/// Thrown because this isn't implemented for <see cref="DiscardStream"/>.
	/// </exception>
	public override int Read (byte[] buffer, int offset, int count)
		=> throw new NotSupportedException();

	/// <summary>
	/// Unimplemented override which is supposed to set the position of the stream.
	/// </summary>
	/// <param name="offset">The position within the stream to seek to (ignored).</param>
	/// <param name="origin">The <see cref="SeekOrigin"/> reference point to seek from (ignored).</param>
	/// <exception cref="NotSupportedException">
	/// Thrown because this isn't implemented for <see cref="DiscardStream"/>.
	/// </exception>
	public override long Seek (long offset, SeekOrigin origin)
		=> throw new NotSupportedException();

	/// <summary>
	/// Unimplemented override which is supposed to set the length of the stream.
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="NotSupportedException">
	/// Thrown because this isn't implemented for <see cref="DiscardStream"/>.
	/// </exception>
	public override void SetLength (long value)
		=> throw new NotSupportedException();

	/// <summary>
	/// Ignores everything being written.
	/// </summary>
	/// <param name="buffer">The data buffer (ignored).</param>
	/// <param name="offset">The offset into the buffer to start writing from (ignored).</param>
	/// <param name="count">The number of bytes to write from the buffer (ignored).</param>
	public override void Write (byte[] buffer, int offset, int count) {} // discard everything
}
