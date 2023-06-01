using System.Net;
using System.Net.Sockets;

using Beinggs.Transfer.Logging;


namespace Beinggs.Transfer.Extensions;


/// <summary>
/// Handy-dandy <see cref="TcpClient"/> and <see cref="Socket"/> extension methods.
/// </summary>
public static class TcpExtensions
{
	/// <summary>
	/// Gets this <see cref="TcpClient"/>'s address.
	/// </summary>
	/// <param name="client">The client to get the address from.</param>
	public static string GetClientAddress (this TcpClient client)
		=> client.Client.RemoteEndPoint is IPEndPoint ep ? ep.Address.ToString() : "unknown";

	/// <summary>
	/// Indicates whether this <see cref="TcpClient"/> is connected.
	/// </summary>
	/// <param name="client">The client to check for connection.</param>
	public static bool IsConnected (this TcpClient client)
		=> client.Connected && client.Client.IsConnected();

	/// <summary>
	/// Indicates whether this <see cref="Socket"/> is connected.
	/// </summary>
	/// <param name="socket">The socket to check for connection.</param>
	public static bool IsConnected (this Socket socket)
	{
		if (socket is null)
			return false;

		try
		{
			// SelectRead is *still true* when socket is closed, so check available too!
			// NOTE: this means if the remote end sends something, this check will fail!!!

			return !socket.Poll (-1, SelectMode.SelectRead) || socket.Available > 0;
		}
		catch (SocketException ex)
		{
			$"Socket poll check failed with: {ex.Message}".Log (LogLevel.Warning);

			return false;
		}
	}
}
