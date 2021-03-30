using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace OGM
{
    class ModuleManager
    {
        private Dictionary<string, object> modules = new Dictionary<string, object>();
        public void Parse()
        {
            foreach(string entry in Directory.GetDirectories("./modules"))
            {
                string meta = Path.Combine(entry, "meta.yml");
                if(!File.Exists(meta))
                    continue;
                string yml = File.ReadAllText(meta);
                Console.WriteLine(yml);
            }
        }
    }
}
