using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocketResponse : Response {

        private string content;
        private DateTime date;
        private HttpStatusCode statusCode;

        private string version = "HTTP/1.1";
        private string server = "Apache / 2.4.16 (Unix) OpenSSL/1.0.2d PHP/5.4.45";
        private string connection = "Upgrade";
        private string upgrade = "websocket";
        private List<HeaderParameter> parameters = new List<HeaderParameter>();

        public WebSocketResponse(DateTime date, HttpStatusCode statusCode) {
            this.date = date;
            this.statusCode = statusCode;
        }

        public string GetContent() {
            return content;
        }

        public DateTime GetDate() {
            return date;
        }

        public HttpStatusCode GetStatus() {
            return statusCode;
        }

        public void SetContent(string content) {
            this.content = content;
        }

        public void SetStatus(HttpStatusCode status) {
            this.statusCode = status;
        }

        public void PutParamter(string key, string value) {
            HeaderParameter parameter = parameters.Find(p => p.Key.ToLower() == key.ToLower());
            if (parameter != null) {
                parameter.Value = value;
            } else {
                parameters.Add(new HeaderParameter(key, value));
            }
        }

        override public string ToString() {
            string message = "";
            message += version + " " + (int)statusCode + " " + statusCode + "\r\n";
            message += "Date: " + date.ToString("ddd, dd MMM yyyy hh:mm:ss " + "GMT") + "\r\n";
            message += "Server: " + server + "\r\n";
            message += "Connection: " + connection + "\r\n";
            parameters.ForEach(p => message += p.Key + ": " + p.Value);
            return message;
        }

    }

}
