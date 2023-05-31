namespace Beinggs.Transfer;


public class DiscardStream: Stream
{
	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override long Length => 0;

	public override long Position { get; set; }

	public override void Flush() {} // nothing to do here

	public override int Read (byte[] buffer, int offset, int count)
		=> throw new NotSupportedException();

	public override long Seek (long offset, SeekOrigin origin)
		=> throw new NotSupportedException();

	public override void SetLength (long value)
		=> throw new NotSupportedException();

	public override void Write (byte[] buffer, int offset, int count) {} // discard everything
}
