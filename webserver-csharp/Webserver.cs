using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace nl.sogyo.webserver {

	public static class Webserver {

		public static void Main() {
			TcpListener listener = new TcpListener(IPAddress.Any, 9090);
			listener.Start();
			while (true) {
				Thread thread = new Thread(new ParameterizedThreadStart(HandleRequest));
				thread.Start(listener.AcceptSocket());
			}
		}

        static void HandleRequest(Object socket) {
            Socket ssocket = socket as Socket;
            WebApplication server = new HttpServer(ssocket);
            do {
                server = server.HandleRequest();
            } while (server?.Connected == true);
            ssocket.Close();
            Console.WriteLine("[DEBUG] Connection closed...");
		}

    }

}

