﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class WebSocketHandShakeResponse : Response {

        private string content;
        private DateTime date;
        private HttpStatusCode statusCode;

        private string version = "HTTP/1.1";
        private string server = "Apache / 2.4.16 (Unix) OpenSSL/1.0.2d PHP/5.4.45";
        private string connection = "Upgrade";
        private string upgrade = "websocket";
        private List<HeaderParameter> headerParameters = new List<HeaderParameter>();

        public WebSocketHandShakeResponse(DateTime date, HttpStatusCode statusCode) {
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

        public List<string> GetHeaderParameterNames() {
            return headerParameters.ConvertAll(p => p.Key);
        }

        public string GetHeaderParameterValue(string name) {
            return headerParameters.Find(p => p.Key.ToLower() == name.ToLower())?.Value;
        }

        public void PutParamter(string key, string value) {
            HeaderParameter parameter = headerParameters.Find(p => p.Key.ToLower() == key.ToLower());
            if (parameter != null) {
                parameter.Value = value;
            } else {
                headerParameters.Add(new HeaderParameter(key, value));
            }
        }

        override public string ToString() {
            string message = "";
            message += version + " " + (int)statusCode + " Switching Protocols\r\n";
            message += "Upgrade: " + upgrade + "\r\n";
            message += "Connection: " + connection + "\r\n";
            headerParameters.ForEach(p => message += p.Key + ": " + p.Value + "\r\n");
            return message;
        }

    }

}
