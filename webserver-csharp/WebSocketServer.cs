using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocketServer : WebApplication {

        private static readonly string MAGIC_STRING = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public bool Connected { get; set; } = false;

        private string secWebSocketKey;
        private string secWebSocketVersion;
        private string secWebSocketAccept;

        public Response Process(Request request) {
            if (!Connected) {
                return GetHandshake(request);
            }



            return null;
        }

        private Response GetHandshake(Request request) {
            secWebSocketKey = request.GetHeaderParameterValue("sec-webSocket-key");
            secWebSocketVersion = request.GetHeaderParameterValue("sec-webSocket-version");
            if (secWebSocketKey == null || secWebSocketVersion != "13")
                return new WebSocketResponseMessage(DateTime.Now, HttpStatusCode.ServerError);
            secWebSocketAccept = GetSocketAccept(secWebSocketKey);
            WebSocketResponseMessage webSocketResponse = new WebSocketResponseMessage(DateTime.Now, HttpStatusCode.SwitchingProtocols);
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
            return accept;//"s3pPLMBiTxaQ9kYGzzhZRbK+xOo=";//
        }

    }

}
