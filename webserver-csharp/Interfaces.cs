using System;
using System.Collections.Generic;

namespace nl.sogyo.webserver
{
	public enum HttpStatusCode {
		OK = 200, NotFound = 404, ServerError = 500
	}

	public enum HttpMethod {GET, POST}

	public interface Request {
		HttpMethod getHTTPMethod();
		string getResourcePath();
		List<string> getHeaderParameterNames();
		String getHeaderParameterValue(String name);
		List<string> getParameterNames();
		string getParameterValue(String name);
	}

	public interface Response {
		HttpStatusCode getStatus();
		void setStatus(HttpStatusCode status);
		DateTime getDate();
		string getContent();
		void setContent(String content);
		string toString();
	}

	public interface WebApplication {
		void process(Request request, Response response);
	}

	public interface Cookie {
		string getName();
		void setName(String name);
		string getValue();
		void setValue(String value);
	}
}

