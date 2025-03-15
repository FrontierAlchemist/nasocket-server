namespace nascoket_server;

internal class Program
{
	private static void Main(string[] args)
	{
		const int Port = 11177;

		new Host(Port).StartListening();
	}
}
