using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class HttpServer : WebApplication {

        public bool Connected { get; set; } = false;

        public Response Process(Request request) {
            HtmlDocument htmlDocument = new HtmlDocument(request);
            return new HttpResponseMessage(htmlDocument.ToString(), DateTime.Now, HttpStatusCode.OK);
        }

    }

}
