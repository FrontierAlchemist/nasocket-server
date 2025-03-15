using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace nascoket_server;

internal class Dialog(Socket clientSocket)
{
	private const string DefaultResponse = "Server recieved message.";

	private readonly Socket clientSocket = clientSocket;

	public void Start()
	{
		var clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
		Console.WriteLine($"Start dialog with client with address {clientEndPoint?.Address}.");
		while (
			clientSocket.Connected &&
			TryReceiveMessage() &&
			TrySendResponse()
		) {
		}
		Console.WriteLine($"Dialog with client with address {clientEndPoint?.Address} is over.");
	}

	private bool TryReceiveMessage()
	{
		try {
			byte[] buffer = new byte[1024];
			int receivedBytes = clientSocket.Receive(buffer);
			if (receivedBytes == 0) {
				Console.WriteLine("Client was close connection.");
				return false;
			}
			string receivedMessage = Encoding.UTF32.GetString(buffer, 0, receivedBytes);
			Console.WriteLine($"Message from client received. Message: \"{receivedMessage}\".");
		} catch (Exception exception) {
			Console.WriteLine($"Error during receiving message from client. Exception message: \"{exception.Message}\".");
			return false;
		}
		return true;
	}

	private bool TrySendResponse(string response = DefaultResponse)
	{
		byte[] responseBytes = Encoding.UTF32.GetBytes(response);
		try {
			clientSocket.Send(responseBytes);
		} catch (Exception exception) {
			Console.WriteLine($"Error during sending response to client. Exception message: \"{exception.Message}\".");
			return false;
		}
		Console.WriteLine($"Response was sent.");
		return true;
	}
}
