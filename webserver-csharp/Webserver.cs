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
        private WebSocketApplication webSocketApplication;

        public WebServer(Socket socket) {
            this.socket = socket;
        }

        public void HandleRequest() {
            try {
                Request request = ReadMessage();
                Response response = Process(request);
                SendMessage(response);
                webSocketApplication?.Start();
            } catch (SocketException ioe) {
                SendMessage(new HttpResponse("", DateTime.Now, HttpStatusCode.ServerError));
                Console.WriteLine("[DEBUG] Error: " + ioe.ToString());
            } finally {
                socket.Close();
            }
        }

        private Request ReadMessage() {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(networkStream);
            string line = "", input = "";
            do {
                line = reader.ReadLine();
                input += line + "\r\n";
                Console.WriteLine(line);
            } while (!string.IsNullOrEmpty(line));
            if (line == null)
                throw new SocketException((int)SocketError.AddressAlreadyInUse);
            reader.Close();
            networkStream.Close();
            return HttpRequest.parse(input);
        }

        private bool IsWebSocketHandshake(Request httpRequest) {
            return httpRequest.GetHeaderParameterValue("connection").ToLower() == "upgrade" &&
                   httpRequest.GetHeaderParameterValue("upgrade").ToLower() == "websocket";
        }

        private Response Process(Request request) {
            return IsWebSocketHandshake(request) ?
                (Response)ProcessHandShakeRequest(request) :
                (Response)ProcessHttpRequest(request);
        }

        private HttpResponse ProcessHttpRequest(Request request) {
            HtmlDocument htmlDocument = null;
            try {
                htmlDocument = new HtmlDocument(request.GetResourcePath());
            } catch (IOException ioe) {
                return new HttpResponse("[404] Resource " + request.GetResourcePath() + " not found", DateTime.Now, HttpStatusCode.NotFound);
            }
            return new HttpResponse(htmlDocument.ToString(), DateTime.Now, HttpStatusCode.OK);
        }

        private WebSocketHandShakeResponse ProcessHandShakeRequest(Request request) {
            int id = this.GetHashCode() % 100;
            webSocketApplication = new WebSocketChat(socket, id);
            return webSocketApplication.Process(request);
        }

        private void SendMessage(Response response) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(networkStream);
            writer.Write(response);
            writer.WriteLine();
            writer.Close();
            networkStream.Close();
        }

        public void OnServerMessage(Response reponse) {
            throw new NotImplementedException();
        }

        public void OnServerMessage(WebSocketFrame frame) {
            webSocketApplication?.SendMessage(frame);
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
            server.HandleRequest();
            DeRegisterServer(server);
            ssocket.Close();
            Console.WriteLine("[DEBUG] Connection closed...");
        }

        static void RegisterServer(WebServer server) {
            lock (serverList) {
                serverList.Add(server);
            }
            WebSocketFrameDispatcher.AddListener(server);
        }

        static void DeRegisterServer(WebServer server) {
            lock (serverList) {
                serverList.Remove(server);
            }
            WebSocketFrameDispatcher.RemoveListener(server);
        }

    }

}

