using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using XTC.oelMVCS;

namespace OGM
{
    class Meta
    {
        public string uri { get; set; }
        public string version { get; set; }
        public string[] load { get; set; }

    }
    class ModuleManager
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
        private static Dictionary<string, Assembly> assemblyMap = new Dictionary<string, Assembly>();
        private static Dictionary<string, string> pathMap = new Dictionary<string, string>();

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
                    foreach (string dll in meta.load)
                    {
                        string assemblyFile = Path.Combine(entry, dll);
                        Assembly assembly = Assembly.LoadFile(assemblyFile);
                        assemblyMap[dll] = assembly;
                        if (dll.EndsWith("winform.dll"))
                        {
                            object instance = assembly.CreateInstance("OGM.Module.File.FormRoot");
                            Type t = assembly.GetType("OGM.Module.File.FormRoot");
                            MethodInfo miInject = t.GetMethod("Inject");
                            miInject.Invoke(instance, new object[] { framework });
                            MethodInfo miRegister = t.GetMethod("Register");
                            miRegister.Invoke(instance, null);
                        }
                        if (dll.EndsWith("module.dll"))
                        {
                            object instance = assembly.CreateInstance("OGM.Module.File.ModuleRoot");
                            Type t = assembly.GetType("OGM.Module.File.ModuleRoot");
                            MethodInfo miInject = t.GetMethod("Inject");
                            miInject.Invoke(instance, new object[] { framework });
                            MethodInfo miRegister = t.GetMethod("Register");
                            miRegister.Invoke(instance, null);
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    logger.Exception(ex);
                }
            }
        }

        public void Cancel()
        {

        }

        private Assembly assemblyResolve(object sender, ResolveEventArgs args)
        {
            return assemblyMap[args.Name.Remove(args.Name.IndexOf(',')) + ".dll"];
        }
    }
}
