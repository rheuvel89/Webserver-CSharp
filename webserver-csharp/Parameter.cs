using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {

    public class Parameter {

        public string Key { get; private set; }
        public string Value { get; private set; }

        private Parameter(string key, string value) {
            this.Key = key;
            this.Value = value;
        }

        public static List<Parameter> Parse(string line) {
            List<Parameter> parameterList = new List<Parameter>();
            string[] parameters = line.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string p in parameters) {
                string[] parts = p.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2) {
                    parameterList.Add(new Parameter(parts[0], parts[1]));
                }
            }
            return parameterList;
        }

    }

}
