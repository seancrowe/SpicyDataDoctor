using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpicyDataDoctor.Logic
{
    public class Document
    {
        public string id;
        public string name;
        public string path;

        public Document(string id, string name, string path)
        {
            this.id = id;
            this.name = name;
            this.path = path;
        }
    }
}
