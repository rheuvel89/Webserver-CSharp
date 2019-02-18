using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.sogyo.webserver {
    
    public class HeaderParameter {

        public string Key { get; private set; }
        public string Value { get; private set; }

        private HeaderParameter(string key, string value) {
            this.Key = key;
            this.Value = value;
        }

        public static HeaderParameter Parse(string line) {
            string[] parts = line.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Count() == 2 ? new HeaderParameter(parts[0], parts[1]) : null;
        }

    }

}
