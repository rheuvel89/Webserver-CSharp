using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class HttpResponseMessage : Response {

        private string content;
        private DateTime date;
        private HttpStatusCode statusCode;

        private string version = "HTTP/1.1";
        private string server = "Apache / 2.4.16 (Unix) OpenSSL/1.0.2d PHP/5.4.45";
        private string connection = "close";
        private string contentType = "text/html; charset=UTF-8";

        public HttpResponseMessage(string content, DateTime date, HttpStatusCode statusCode) {
            this.content = content;
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

        override public string ToString() {
            string message = "";
            message += version + " " + (int)statusCode + " " + statusCode + "\r\n";
            message += "Date: " + date.ToString("ddd, dd MMM yyyy hh:mm:ss " + "GMT") + "\r\n";
            message += "Server: " + server + "\r\n";
            message += "Connection: " + connection + "\r\n";
            message += "Content-Type: " + contentType + "\r\n";
            message += content.Length > 0 ? "Content-length: " + content.Length + "\r\n" : "";
            message += "\r\n" + content;
            return message;
        }

    }

}
