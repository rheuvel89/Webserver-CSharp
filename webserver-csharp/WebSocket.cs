using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocket : WebApplication {

        private static readonly string MAGIC_STRING = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public bool Connected { get; private set; } = false;
                
        private string secWebSocketKey;
        private string secWebSocketVersion;
        private string secWebSocketAccept;

        public Response GetHandshake(Request request) {
            secWebSocketKey = request.GetHeaderParameterValue("sec-webSocket-key");
            secWebSocketVersion = request.GetHeaderParameterValue("sec-webSocket-version");
            if (secWebSocketKey == null || secWebSocketVersion != "13")
                return new WebSocketResponse(DateTime.Now, HttpStatusCode.ServerError);
            secWebSocketAccept = GetSocketAccept(secWebSocketKey);
            WebSocketResponse webSocketResponse = new WebSocketResponse(DateTime.Now, HttpStatusCode.SwitchingProtocols);
            webSocketResponse.PutParamter("Sec-WebSocket-Accept", secWebSocketAccept);
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

        public void Process(Request request, Response response) {
            throw new NotImplementedException();
        }

    }

}
