using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SpicyDataDoctor.Logic
{
    public class Asset
    {
        public string id;
        public string name;
        public string path;

        public Asset(string id, string name, string path)
        {
            this.id = id;
            this.name = name;
            this.path = path;
        }

    }
}
