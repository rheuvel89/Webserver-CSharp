using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace nl.sogyo.webserver {

	public static class Webserver {

		public static void Main() {
			TcpListener listener = new TcpListener(9090);
			listener.Start();
			while (true) {
				Thread thread = new Thread(new ParameterizedThreadStart(HandleRequest));
				thread.Start(listener.AcceptSocket());
			}
		}

        static void HandleRequest(Object socket) {
            Socket ssocket = socket as Socket;
            WebSocketServer webSocketServer = null;
            WebApplication server = null;
            do {
                Request request = ReadMessage(ssocket);
                server = SelectServer(webSocketServer, request);
                webSocketServer = server is WebSocketServer ? (WebSocketServer)server : webSocketServer;
                Response response = server.Process(request);
                SendMessage(ssocket, response); 
            } while (server.Connected);
            ssocket.Close();
            Console.WriteLine("[DEBUG] Connection closed...");
		}

        static RequestMessage ReadMessage(Socket socket) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(networkStream);
            string line = null;
            string input = "";
            while (line != "") {
                line = reader.ReadLine();
                input += line + "\r\n";
                Console.WriteLine(line);
            }
            reader.Close();
            networkStream.Close();
            return RequestMessage.parse(input); ;
        }

        static WebApplication SelectServer(WebSocketServer webSocketServer, Request httpRequest) {
            bool isWebSocketRequest = httpRequest.GetHeaderParameterValue("connection").ToLower() == "upgrade" &&
                                      httpRequest.GetHeaderParameterValue("upgrade").ToLower() == "websocket";
            if (isWebSocketRequest) {
                return webSocketServer != null && webSocketServer.Connected ? webSocketServer : new WebSocketServer();
            } else {
                return new HttpServer();
            }
        }

        static void SendMessage(Socket socket, Response response) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(networkStream);
            writer.Write(response);
            writer.WriteLine();
            writer.Close();
            networkStream.Close();
        }

	}

}

