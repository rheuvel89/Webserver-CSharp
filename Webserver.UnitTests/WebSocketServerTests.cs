using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nl.sogyo.webserver.UnitTests {

    [TestClass]
    public class WebSocketServerTests {

        [TestMethod]
        public void GetSocketAccept() {
            string input = "GET / HTTP/1.1\r\nOrigin: http://localhost:9090\r\nSec-WebSocket-Key: dGhlIHNhbXBsZSBub25jZQ==\r\nConnection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Version: 13\r\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/17.17134\r\nHost: localhost:9090\r\nCache-Control: no-cache\r\n\r\n";
            HttpRequestMessage request = HttpRequestMessage.parse(input);
            WebSocketServer webSocketServer = new WebSocketServer();

            WebSocketHandShakeMessage response = (WebSocketHandShakeMessage)webSocketServer.Process(request);

            Assert.AreEqual("s3pPLMBiTxaQ9kYGzzhZRbK+xOo=", response.GetHeaderParameterValue("Sec-WebSocket-Accept"));
        }

    }

}
