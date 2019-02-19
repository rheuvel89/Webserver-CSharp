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
                server = TryHandleRequest(ssocket, server, webSocketServer);
                webSocketServer = server is WebSocketServer ? (WebSocketServer)server : webSocketServer;
            } while (server != null && server.Connected);
            ssocket.Close();
            Console.WriteLine("[DEBUG] Connection closed...");
		}

        static WebApplication TryHandleRequest(Socket ssocket, WebApplication server, WebSocketServer webSocketServer) {
            try {
                Request request = ReadMessage(ssocket);
                server = SelectServer(webSocketServer, request);
                Response response = server.Process(request);
                SendMessage(ssocket, response);
            } catch (SocketException e) {
                SendMessage(ssocket, new HttpResponseMessage("", DateTime.Now, HttpStatusCode.ServerError));
                Console.WriteLine("[DEBUG] Error: " + e.ToString());
            }
            return server;
        }

        static RequestMessage ReadMessage(Socket socket) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(networkStream);
            string line = "";
            string input = "";
            do {
                line = reader.ReadLine();
                input += line + "\r\n";
                Console.WriteLine(line);
            } while (!string.IsNullOrEmpty(line));
            if (line == null)
                throw new SocketException((int)SocketError.Fault);
            reader.Close();
            networkStream.Close();
            return RequestMessage.parse(input); ;
        }

        static WebApplication SelectServer(WebSocketServer webSocketServer, Request httpRequest) {
            bool isWebSocketRequest = httpRequest.GetHeaderParameterValue("connection").ToLower() == "upgrade" &&
                                      httpRequest.GetHeaderParameterValue("upgrade").ToLower() == "websocket";
            if (isWebSocketRequest) {
                return webSocketServer?.Connected == true ? webSocketServer : new WebSocketServer();
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

