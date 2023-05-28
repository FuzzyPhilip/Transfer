using System.Net;
using System.Net.Sockets;


namespace Beinggs.Transfer.Extensions;


internal static class TcpExtensions
{
	public static string GetClientAddress (this TcpClient client)
		=> client.Client.RemoteEndPoint is IPEndPoint ep ? ep.Address.ToString() : "unknown";

	public static bool IsConnected (this TcpClient tcpClient)
		=> tcpClient.Connected && tcpClient.Client.IsConnected();

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
