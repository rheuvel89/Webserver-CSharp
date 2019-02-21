﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class HttpServer : WebApplication {

        public bool Connected { get; set; } = false;
        private Socket socket;

        public HttpServer(Socket socket) {
            this.socket = socket;
        }

        public WebApplication HandleRequest() {
            WebApplication server = null;
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

        public WebApplication SelectServer(Request httpRequest) {
            bool isWebSocketRequest = httpRequest.GetHeaderParameterValue("connection").ToLower() == "upgrade" &&
                                      httpRequest.GetHeaderParameterValue("upgrade").ToLower() == "websocket";
            return isWebSocketRequest ? new WebSocketServer(socket) : (WebApplication)this;
        }

        public void SendMessage(Response response) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(networkStream);
            writer.Write(response);
            writer.WriteLine();
            writer.Close();
            networkStream.Close();
        }

        public Response Process(Request request) {
            HtmlDocument htmlDocument = new HtmlDocument(request);
            return new HttpResponseMessage(htmlDocument.ToString(), DateTime.Now, HttpStatusCode.OK);
        }

    }

}
