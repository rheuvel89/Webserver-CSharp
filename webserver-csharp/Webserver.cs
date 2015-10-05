using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace nl.sogyo.webserver
{
	public static class Webserver
	{
		public static void Main()
		{
			TcpListener listener = new TcpListener(9090);
			listener.Start();
			while (true)
			{
				Thread thread = new Thread(new ParameterizedThreadStart(HandleRequest));
				thread.Start(listener.AcceptSocket());
			}
		}

		static void HandleRequest(Object socket)
		{
			Socket ssocket = socket as Socket;
			NetworkStream networkstream = new NetworkStream(ssocket);
			StreamReader reader = new StreamReader(networkstream);
			string input=null;
			//read the input until a blank line is read
			while (input != "")
			{
				input = reader.ReadLine();
				Console.WriteLine(input);
			}
			reader.Close();
			networkstream = new NetworkStream(ssocket);
			StreamWriter writer = new StreamWriter(networkstream);
			writer.WriteLine("Thank you for connecting!");
			writer.WriteLine();
			writer.Close();
			networkstream.Close();
			ssocket.Close();
		}
	}
}

