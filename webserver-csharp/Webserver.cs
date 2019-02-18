using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace nl.sogyo.webserver
{
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
			NetworkStream networkstream = new NetworkStream(ssocket);
			StreamReader reader = new StreamReader(networkstream);
			string line=null;
            string input = "";
			//read the input until a blank line is read
			while (line != "") {
				line = reader.ReadLine();
                input += line + "\r\n";
				Console.WriteLine(line);
			}
			reader.Close();
            RequestMessage httpRequest = RequestMessage.parse(input);

            ResponseMessage response = ProcessNormalRequest(httpRequest);

			networkstream = new NetworkStream(ssocket);
			StreamWriter writer = new StreamWriter(networkstream);
			writer.Write(response);

			writer.WriteLine();
			writer.Close();
			networkstream.Close();
			ssocket.Close();
		}

        static ResponseMessage ProcessNormalRequest(RequestMessage httpRequest) {
            HtmlDocument htmlDocument = new HtmlDocument(httpRequest);
            return new ResponseMessage(htmlDocument.ToString(), DateTime.Now, HttpStatusCode.OK);
        }
	}
}

