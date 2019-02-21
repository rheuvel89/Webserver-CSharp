using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace nl.sogyo.webserver
{
	public enum HttpStatusCode {
        SwitchingProtocols = 101, OK = 200, NotFound = 404, ServerError = 500
	}

	public enum HttpMethod {GET, POST}

	public interface Request {
		HttpMethod GetHTTPMethod();
		string GetResourcePath();
		List<string> GetHeaderParameterNames();
		String GetHeaderParameterValue(String name);
		List<string> GetParameterNames();
		string GetParameterValue(String name);
	}

	public interface Response {
		HttpStatusCode GetStatus();
		void SetStatus(HttpStatusCode status);
		DateTime GetDate();
		string GetContent();
		void SetContent(String content);
		string ToString();
	}

	public interface WebApplication {
        bool Connected { get; set; }
        WebApplication HandleRequest();
        Response Process(Request request);
    }

	public interface Cookie {
		string getName();
		void setName(String name);
		string getValue();
		void setValue(String value);
	}
}

