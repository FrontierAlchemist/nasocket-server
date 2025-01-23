using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NasocketServer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			const int Port = 11177;

			IPEndPoint endPoint = new(IPAddress.Any, Port);
			Socket serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try {
				serverSocket.Bind(endPoint);
				serverSocket.Listen();
				while (true) {
					Console.WriteLine("Waiting for client...");
					Socket clientSocket = serverSocket.Accept();
					var clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
					Console.WriteLine($"Client {clientEndPoint?.Address} is connected");
					try {
						while (clientSocket.Connected) {
							byte[] buffer = new byte[1024];
							int recievedBytes = clientSocket.Receive(buffer);
							if (recievedBytes == 0) {
								Console.WriteLine("Client was close connection");
								break;
							}
							string recievedMessage = Encoding.UTF32.GetString(buffer, 0, recievedBytes);
							Console.WriteLine($"Message from client: {recievedMessage}");

							string response = "Server recieved message";
							byte[] responseBytes = Encoding.UTF32.GetBytes(response);
							clientSocket.Send(responseBytes);
							Console.WriteLine($"Response was sent");
						}
					} catch (Exception exception) {
						Console.WriteLine($"Error! {exception.Message}");
					} finally {
						clientSocket.Close();
					}
				}
			} catch (Exception exception) {
				Console.WriteLine($"Fatal error! {exception.Message}. Server stoped");
			} finally {
				serverSocket.Close();
			}
		}
	}
}
