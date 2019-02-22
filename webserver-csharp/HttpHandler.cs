using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class HttpHandler : WebServerHandler {

        public bool Connected { get; set; } = false;
        private Socket socket;

        public HttpHandler(Socket socket) {
            this.socket = socket;
        }

        public WebServerHandler HandleRequest() {
            WebServerHandler server = null;
            try {
                Request request = ReadMessage();
                server = SelectServer(request);
                Response response = server.Process(request);
                SendMessage(response);
            } catch (SocketException e) {
                SendMessage(new HttpResponseMessage("", DateTime.Now, HttpStatusCode.ServerError));
                Console.WriteLine("[DEBUG] Error: " + e.ToString());
            }
            return server;
        }

        public Request ReadMessage() {
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
                throw new SocketException((int)SocketError.AddressAlreadyInUse);
            reader.Close();
            networkStream.Close();
            return HttpRequestMessage.parse(input);
        }

        public WebServerHandler SelectServer(Request httpRequest) {
            bool isWebSocketRequest = httpRequest.GetHeaderParameterValue("connection").ToLower() == "upgrade" &&
                                      httpRequest.GetHeaderParameterValue("upgrade").ToLower() == "websocket";
            if (isWebSocketRequest) {
                int id = this.GetHashCode() % 100;
                return new WebSocketHandler(socket, id);
            } else {
                return this;
            }
        }

        public Response Process(Request request) {
            HtmlDocument htmlDocument = null;
            try {
                htmlDocument = new HtmlDocument(request.GetResourcePath());
                //htmlDocument = new HtmlDocument(request);
            } catch (IOException ioe) {
                return new HttpResponseMessage("[404] Resource " + request.GetResourcePath() + " not found", DateTime.Now, HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage(htmlDocument.ToString(), DateTime.Now, HttpStatusCode.OK);
        }

        public void SendMessage(Response response) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(networkStream);
            writer.Write(response);
            writer.WriteLine();
            writer.Close();
            networkStream.Close();
        }

    }

}
