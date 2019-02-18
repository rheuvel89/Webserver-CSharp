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
            string input = ReadMessage(ssocket);
            RequestMessage httpRequest = RequestMessage.parse(input);
            ResponseMessage response = ProcessNormalRequest(httpRequest);
            SendMessage(ssocket, response);
			ssocket.Close();
		}

        static string ReadMessage(Socket socket) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamReader reader = new StreamReader(networkStream);
            string line = null;
            string input = "";
            //read the input until a blank line is read
            while (line != "") {
                line = reader.ReadLine();
                input += line + "\r\n";
                Console.WriteLine(line);
            }
            reader.Close();
            networkStream.Close();
            return input;
        }

        static ResponseMessage ProcessNormalRequest(RequestMessage httpRequest) {
            HtmlDocument htmlDocument = new HtmlDocument(httpRequest);
            return new ResponseMessage(htmlDocument.ToString(), DateTime.Now, HttpStatusCode.OK);
        }

        static void SendMessage(Socket socket, ResponseMessage response) {
            NetworkStream networkStream = new NetworkStream(socket);
            StreamWriter writer = new StreamWriter(networkStream);
            writer.Write(response);
            writer.WriteLine();
            writer.Close();
            networkStream.Close();
        }

	}

}

