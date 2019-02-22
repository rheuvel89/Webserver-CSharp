using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;
using System.Collections.Generic;

namespace nl.sogyo.webserver {

	public class WebServer : ServerHandlerListener {

        private static List<WebServer> serverList = new List<WebServer>();

        private Socket socket;
        private WebServerHandler webServerHandler;

        public WebServer(Socket socket) {
            this.socket = socket;
            webServerHandler = new HttpHandler(socket);
        }

        public void HandleRequests() {
            try {
                do {
                    webServerHandler = webServerHandler.HandleRequest();
                } while (webServerHandler?.Connected == true);
            } catch (IOException ioe) {
                Console.WriteLine("[DEBUG] Client allready disconnected...");
            } finally {
                socket.Close();
            }
        }

        public void OnServerMessage(Response reponse) {
            throw new NotImplementedException();
        }

        public void OnServerMessage(WebSocketFrame frame) {
            ((WebSocketHandler)webServerHandler).SendMessage(frame);
        }

        public static void Main() {
			TcpListener listener = new TcpListener(IPAddress.Any, 9090);
			listener.Start();
			while (true) {
				Thread thread = new Thread(new ParameterizedThreadStart(HandleRequests));
				thread.Start(listener.AcceptSocket());
			}
		}

        static void HandleRequests(object socket) {
            Socket ssocket = socket as Socket;
            WebServer server = new WebServer(ssocket);
            RegisterServer(server);
            server.HandleRequests();
            DeRegisterServer(server);
            ssocket.Close();
            Console.WriteLine("[DEBUG] Connection closed...");
        }

        static void RegisterServer(WebServer server) {
            lock (serverList) {
                serverList.Add(server);
            }
            MessageDispatcher.AddListener(server);
        }

        static void DeRegisterServer(WebServer server) {
            lock (serverList) {
                serverList.Remove(server);
            }
            MessageDispatcher.RemoveListener(server);
        }

    }

}

