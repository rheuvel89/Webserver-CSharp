using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocketHandler : WebServerHandler {

        private static readonly string MAGIC_STRING = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public bool Connected { get; set; } = false;

        private int id;

        private Socket socket;
        private string secWebSocketKey;
        private string secWebSocketVersion;
        private string secWebSocketAccept;

        public WebSocketHandler(Socket socket, int id) {
            this.id = id;
            this.socket = socket;
        }

        public WebServerHandler HandleRequest() {
            WebSocketFrame incomingFrame = ReadMessage();
            WebSocketFrame outgoingFrame = Process(incomingFrame);
            if (outgoingFrame != null) {
                MessageDispatcher.Dispatch(outgoingFrame);
            }
            return this;
        }

        public WebSocketFrame ReadMessage() {
            NetworkStream networkStream = new NetworkStream(socket);
            byte[] frame = new byte[256];
            networkStream.Read(frame, 0, 2);
            bool isMaskSet = WebSocketFrame.IsMaskSet(frame);
            int index = 2;
            int remainingByes = (isMaskSet ? 4 : 0) + frame[1] & 0b01111111;
            if ((frame[1] & 0b01111111) == 126) {
                networkStream.Read(frame, index, 2);
                remainingByes = (isMaskSet ? 4 : 0) + BitConverter.ToInt16(frame, 2);
                index += 2;
            } else if ((frame[1] & 0b01111111) == 127) {
                networkStream.Read(frame, index, 8);
                remainingByes = (isMaskSet ? 4 : 0) + BitConverter.ToInt32(frame, 2);
                index += 8;
                throw new NotImplementedException();
            }
            networkStream.Read(frame, index, remainingByes);
            networkStream.Close();
            return WebSocketFrame.Parse(frame);
        }

        public Response Process(Request request) {
            return GetHandshake(request);
        }
        public WebSocketFrame Process(WebSocketFrame webSocketFrame) {
            WebSocketFrame response = null;
            switch (webSocketFrame.OpCode) {
                case 1: response = new WebSocketFrame("[" + id + "]: " + webSocketFrame.AsString()); break;
                case 8: Connected = false; break;
            }
            return response;
        }


        public void SendMessage(WebSocketFrame webSocketFrame) {
            NetworkStream networkStream = new NetworkStream(socket);
            networkStream.Write(webSocketFrame.Frame, 0, webSocketFrame.Size);
            networkStream.Close();
        }

        private Response GetHandshake(Request request) {
            secWebSocketKey = request.GetHeaderParameterValue("sec-webSocket-key");
            secWebSocketVersion = request.GetHeaderParameterValue("sec-webSocket-version");
            if (secWebSocketKey == null || secWebSocketVersion != "13")
                return new WebSocketHandShakeMessage(DateTime.Now, HttpStatusCode.ServerError);
            secWebSocketAccept = GetSocketAccept(secWebSocketKey);
            WebSocketHandShakeMessage webSocketResponse = new WebSocketHandShakeMessage(DateTime.Now, HttpStatusCode.SwitchingProtocols);
            webSocketResponse.PutParamter("Sec-WebSocket-Accept", secWebSocketAccept);
            //webSocketResponse.PutParamter("Sec-WebSocket-Protocol", "chat");
            Connected = true;
            return webSocketResponse;
        }

        private string GetSocketAccept(string key) {
            string accept = key + MAGIC_STRING;
            using (SHA1Managed sha1 = new SHA1Managed()) {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(accept));
                accept = Convert.ToBase64String(hash);
            }
            return accept;
        }

        public void SendMessage(Response response) {
            throw new NotImplementedException();
        }

    }

}
