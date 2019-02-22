using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class HtmlDocument {

        string content =  "";

        public HtmlDocument(Request request) {
            AddLine("You did an HTTP GET request.");
            AddLine("Requested resource: " + request.GetResourcePath());
            AddLine();
            AddLine("The following header parameters where passed:");
            request.GetHeaderParameterNames().ForEach(p => AddLine(p + ": " + request.GetHeaderParameterValue(p)));
            AddLine();
            AddLine("The following parameters were passed:");
            request.GetParameterNames().ForEach(p => AddLine(p + ": " + request.GetParameterValue(p)));
        }

        public HtmlDocument(string path) {
            FileStream htmlFile = null;
            StreamReader reader = null;
            try {
                path = Directory.GetCurrentDirectory() + path;
                htmlFile = File.OpenRead(path);
                reader = new StreamReader(htmlFile);
                content = reader.ReadToEnd();
            } finally {
                reader?.Close();
                htmlFile?.Close();
            }
        }

        public void AddLine(string line) {
            content += "<br/>\r\n" + line;
        }

        public void AddLine() {
            AddLine("");
        }

        public override string ToString() {
            return "<html>\r\n<body>" + content + "\r\n</html>\r\n</body>";
        }

    }

}
