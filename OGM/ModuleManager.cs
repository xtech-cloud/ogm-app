using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using XTC.oelMVCS;

namespace OGM
{
    class Meta
    {
        public class Library
        {
            public string file { get; set; }
            public string entry { get; set; }
        }
        public string uri { get; set; }
        public string path { get; set; }
        public string version { get; set; }
        public Library[] load { get; set; }

    }
    public class ModuleManager
    {
        public ConsoleLogger logger { get; set; }
        public Framework framework { get; set; }

        public class Module
        {
            private object instance = null;
            private MethodInfo mi = null;
            private object[] param = null;

            public Module(object _instance, MethodInfo _methodInfo)
            {
                instance = _instance;
                mi = _methodInfo;
                param = new object[2];
            }

            public void Invoke(string _method, object[] _param)
            {
                param[0] = _method;
                param[1] = _param;
                mi.Invoke(instance, param);
            }
        }
        private Dictionary<string, Assembly> assemblyMap = new Dictionary<string, Assembly>();
        private Dictionary<string, string> pathMap = new Dictionary<string, string>();

        public ModuleManager()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(assemblyResolve);
        }

        public void Register()
        {
            string curDir = System.IO.Directory.GetCurrentDirectory();
            foreach (string entry in Directory.GetDirectories(Path.Combine(curDir,"modules")))
            {
                string metafile = Path.Combine(entry, "meta.json");
                if(!File.Exists(metafile))
                    continue;
                string json  = File.ReadAllText(metafile);
                try
                {
                    Meta meta = System.Text.Json.JsonSerializer.Deserialize<Meta>(json);
                    foreach (Meta.Library library in meta.load)
                    {
                        string assemblyFile = Path.Combine(entry, library.file);
                        Assembly assembly = Assembly.LoadFile(assemblyFile);
                        assemblyMap[library.file] = assembly;
                        if (!string.IsNullOrEmpty(library.entry))
                        {
                            object instance = assembly.CreateInstance(library.entry);
                            Type t = assembly.GetType(library.entry);
                            MethodInfo miInject = t.GetMethod("Inject");
                            miInject.Invoke(instance, new object[] { framework });
                            MethodInfo miRegister = t.GetMethod("Register");
                            miRegister.Invoke(instance, null);
                        }
                    }
                    pathMap[meta.uri] = meta.path;
                }
                catch (System.Exception ex)
                {
                    logger.Exception(ex);
                }
            }
        }

        public void Cancel()
        {

        }

        public string convertPath(string _uri)
        {
           foreach(string uri in pathMap.Keys)
           {
                if(_uri.StartsWith(uri))
                {
                    return _uri.Replace(uri, pathMap[uri]);
                }
           }
            return _uri;
        }

        private Assembly assemblyResolve(object sender, ResolveEventArgs args)
        {
            return assemblyMap[args.Name.Remove(args.Name.IndexOf(',')) + ".dll"];
        }
    }
}
