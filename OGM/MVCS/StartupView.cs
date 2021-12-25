using ogm.account;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using XTC.oelMVCS;

namespace OGM
{
    public class StartupView : View
    {
        public class StartupViewBridge : IStartupViewBridge
        {
            public StartupView view { get; set; }

            public void OnSettingSubmit(Dictionary<string, Any> _settings)
            {
                (view.getConfig() as AppConfig).MergeDict(_settings);

                var options = new JsonSerializerOptions();
                options.Converters.Add(new AnyProtoConverter());
                var json = JsonSerializer.Serialize(_settings, options);

                string curDir = System.IO.Directory.GetCurrentDirectory();
                string configsDir = Path.Combine(curDir, "configs");
                string filepath = Path.Combine(configsDir, "app.json");

                File.WriteAllText(filepath, json);
            }

            public void OnSigninSubmit(string _username, string _password)
            {
                var service = view.findService(ogm.account.AuthService.NAME) as ogm.account.AuthService;
                var req = new ogm.account.Proto.SigninRequest();
                req._username = Any.FromString(_username);
                req._password = Any.FromString(_password);
                req._strategy = Any.FromInt32(1);
                service.PostSignin(req);
            }
        }

        public const string NAME = "StartupView";

        private Facade myFacade = null;

        protected override void preSetup()
        {
            myFacade = findFacade(StartupFacade.NAME);
            StartupViewBridge vb = new StartupViewBridge();
            vb.view = this;
            myFacade.setViewBridge(vb);
        }

        protected override void postSetup()
        {
            addRouter("/Application/Auth/Signin/Success", handleAuthSigninSuccess);
            addRouter("/Application/Auth/Signin/Failure", handleAuthSigninFailure);

            Dictionary<string, Any> settings = new Dictionary<string, Any>();
            if (getConfig().Has("domain"))
                settings["domain"] = getConfig().getField("domain");
            if (getConfig().Has("apikey"))
                settings["apikey"] = getConfig().getField("apikey");
            var bridge = myFacade.getUiBridge() as IStartupUiBridge;
            bridge.HandleSettingLoad(settings);
        }

        private void handleAuthSigninSuccess(Model.Status _satus, object _data)
        {
            var rsp = _data as Dictionary<string, Any>;
            var bridge = myFacade.getUiBridge() as IStartupUiBridge;
            bridge.HandleSignin(rsp["code"].AsInt32(), rsp["message"].AsString());
        }

        private void handleAuthSigninFailure(Model.Status _satus, object _data)
        {
            var rsp = _data as Dictionary<string, Any>;
            var bridge = myFacade.getUiBridge() as IStartupUiBridge;
            bridge.HandleSignin(rsp["code"].AsInt32(), rsp["message"].AsString());
        }

    }
}
