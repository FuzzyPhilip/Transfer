namespace Beinggs.Transfer;


/// <summary>
/// Provides randomised test data as a read-only <see cref="Stream"/>.
/// </summary>
public class ReadOnlyTestStream: Stream
{
	const int _randomDataLength = 65536;

	static readonly Random _rnd = new ((int) DateTime.Now.Ticks);
	static readonly byte[] _randomBuffer = new byte [_randomDataLength];
	static readonly Memory<byte> _randomData = new (_randomBuffer);

	readonly long _length;
	long _position;

	/// <summary>
	/// Always returns <see langword="true"/> for a test stream.
	/// </summary>
	public override bool CanRead => true;

	/// <summary>
	/// Always returns <see langword="true"/> for a test stream.
	/// </summary>
	public override bool CanSeek => true;

	/// <summary>
	/// Always returns <see langword="false"/> for a test stream.
	/// </summary>
	public override bool CanWrite => false;

	/// <summary>
	/// Gets the length of the test stream, given at construction time.
	/// </summary>
	public override long Length => _length;

	/// <summary>
	/// Gets or sets the position of the test stream within its <see cref="Length"/>.
	/// </summary>
	public override long Position
	{
		get => _position;

		set => _position = value < 0 || value >= _length
			? throw new ArgumentException ("The position must be set to somewhere within the stream's length")
			: value;
	}

	static ReadOnlyTestStream() // pre-fill the buffer with some random data (don't want to generate it at read time)
		=> _rnd.NextBytes (_randomBuffer);

	/// <summary>
	/// Creates a new instance of a <see cref="ReadOnlyTestStream"/>.
	/// </summary>
	/// <param name="length"></param>
	public ReadOnlyTestStream (long length)
		=> _length = length;

	/// <summary>
	/// Flushes the stream (does nothing in a test stream).
	/// </summary>
	public override void Flush() {} // nothing to do for test stream

	/// <summary>
	/// Writes the requested number of random bytes into the given buffer, or fewer if near the end of the stream.
	/// </summary>
	/// <remarks>
	/// Note: This operation will always return the same 'random' data as it uses a static buffer which is filled with
	/// random data on first use.
	/// and filled once to improve test stream read performance.
	/// </remarks>
	/// <param name="buffer"></param>
	/// <param name="offset"></param>
	/// <param name="count"></param>
	/// <returns>
	/// The actual number of bytes 'read', or zero if the test stream's position is at the end of the stream.
	/// </returns>
	public override int Read (byte[] buffer, int offset, int count)
	{
		var toRead = (int) Math.Min (count, _length - _position);

		if (toRead <= 0)
			return 0;

		var chunks = count / _randomDataLength;
		var lastChunk = count % _randomDataLength;

		for (var i = 0; i < chunks; i++, offset += _randomDataLength)
			_randomData.CopyTo (new Memory<byte> (buffer, offset, _randomDataLength));

		if (lastChunk > 0)
			_randomData.CopyTo (new Memory<byte> (buffer, offset, lastChunk));

		_position += toRead;

		return toRead;
	}

	/// <summary>
	/// Sets the <see cref="Position"/> to the given offset relative to the given origin.
	/// </summary>
	/// <param name="offset"></param>
	/// <param name="origin"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public override long Seek (long offset, SeekOrigin origin)
		=> Position = origin switch
		{
			SeekOrigin.Begin => offset,
			SeekOrigin.Current => _position + offset,
			SeekOrigin.End => _length + offset,

			_ => throw new ArgumentException ($"Invalid {nameof (SeekOrigin)} value given")
		};

	/// <summary>
	/// Throws an <see cref="InvalidOperationException"/>, as a test stream's length can't be changed.
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public override void SetLength (long value)
		=> throw new InvalidOperationException ("A test stream's length can't be changed");

	/// <summary>
	/// Throws an <see cref="InvalidOperationException"/>, as a test stream can't be written to.
	/// </summary>
	/// <param name="buffer"></param>
	/// <param name="offset"></param>
	/// <param name="count"></param>
	/// <exception cref="InvalidOperationException"></exception>
	public override void Write (byte[] buffer, int offset, int count)
		=> throw new InvalidOperationException();
}

