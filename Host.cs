using System;
using System.Net;
using System.Net.Sockets;

namespace nascoket_server;

internal class Host(int port)
{
	private readonly IPEndPoint endPoint = new(IPAddress.Any, port);
	private readonly Socket serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

	public void StartListening()
	{
		if (!TryPrepareListening()) {
			return;
		}

		Socket? clientSocket;
		while (WaitClient(out clientSocket)) {
			if (clientSocket != null) {
				StartDialog(clientSocket);
			}
			clientSocket?.Close();
		}
		clientSocket?.Close();
	}

	private bool TryPrepareListening()
	{
		try {
			serverSocket.Bind(endPoint);
			serverSocket.Listen();
		} catch (Exception exception) {
			Console.WriteLine($"Can't start listening. Exception message: {exception.Message}.");
			return false;
		}
		return true;
	}

	private bool WaitClient(out Socket? clientSocket)
	{
		try {
			Console.WriteLine("Waiting for client...");
			clientSocket = serverSocket.Accept();
		} catch (Exception exception) {
			Console.WriteLine($"Error during waiting for client. Exception message: {exception.Message}.");
			clientSocket = null;
			return false;
		}
		var clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
		Console.WriteLine($"Client with address {clientEndPoint?.Address} is connected.");
		return true;
	}

	private static void StartDialog(Socket clientSocket) => new Dialog(clientSocket).Start();
}
