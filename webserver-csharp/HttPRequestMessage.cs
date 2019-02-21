using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class HttpRequestMessage : Request {

        private string request, resourceParameter, version;
        private HttpMethod httpMethod;
        private List<HeaderParameter> headerParameters = new List<HeaderParameter>();
        private List<Parameter> parameters = new List<Parameter>();   

        public static HttpRequestMessage parse(string request) {
            return new HttpRequestMessage(request);
        }

        private HttpRequestMessage(string request) {
            this.request = request;
            int headerLineEnd = request.IndexOf("\r\n") + 2;
            int headerParametersEnd = request.IndexOf("\r\n\r\n") + 2;
            ParseHeaderLine(request.Substring(0, headerLineEnd));
            ParseHeaderParameters(request.Substring(headerLineEnd, headerParametersEnd - headerLineEnd));
            ParseParameters(request.Substring(headerParametersEnd));
        }

        private void ParseHeaderLine(string headerLine) {
            string[] parts = headerLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts[0] == "GET") {
                httpMethod = HttpMethod.GET;
            } else if (parts[0] == "POST") {
                httpMethod = HttpMethod.POST;
            }
            resourceParameter = parts[1];
            version = parts[2];
        }

        private void ParseHeaderParameters(string headerParameterLines) {
            string[] lines = headerParameterLines.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines) {
                HeaderParameter parameter = HeaderParameter.Parse(line);
                if (parameter != null) {
                    headerParameters.Add(parameter);
                }
            }
        }

        private void ParseParameters(string body) {
            string parameterString = httpMethod == HttpMethod.GET ? GetParametersForGET() : body;
            parameters.AddRange(Parameter.Parse(parameterString));
        }

        private string GetParametersForGET() {
            int index = resourceParameter.IndexOf('?');
            return index >= 0 ? resourceParameter.Substring(index + 1) : "";
        }

        public List<string> GetHeaderParameterNames() {
            return headerParameters.ConvertAll(p => p.Key);
        }

        public string GetHeaderParameterValue(string name) {
            return headerParameters.Find(p => p.Key.ToLower() == name.ToLower())?.Value;
        }

        public HttpMethod GetHTTPMethod() {
            return httpMethod;
        }

        public List<string> GetParameterNames() {
            return parameters.ConvertAll(p => p.Key);
        }

        public string GetParameterValue(string name) {
            return parameters.Find(p => p.Key.ToLower() == name.ToLower())?.Value;
        }

        public string GetResourcePath() {
            int index = resourceParameter.IndexOf('?');
            return index >= 0 ? resourceParameter.Substring(0, index) : resourceParameter;
        }
    }

}
