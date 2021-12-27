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
                var service = view.findService(AuthService.NAME) as AuthService;
                var req = new Proto.SigninRequest();
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
            addRouter("/Application/Auth/Signin", handleAuthSignin);
            addRouter("/Application/Auth/Success", handleAuthSuccess);
            addRouter("/Application/Auth/Failure", handleAuthFailure);
            addRouter("/Application/Auth/Where", handleAuthWhere);
            addRouter("/Application/Auth/Role", handleAuthRole);
            addRouter("/Application/Auth/Permission", handleAuthPermission);

            Dictionary<string, Any> settings = new Dictionary<string, Any>();
            if (getConfig().Has("domain"))
                settings["domain"] = getConfig().getField("domain");
            if (getConfig().Has("apikey"))
                settings["apikey"] = getConfig().getField("apikey");
            var bridge = myFacade.getUiBridge() as IStartupUiBridge;
            bridge.HandleSettingLoad(settings);
        }

        private void handleAuthSignin(Model.Status _status, object _data)
        {
            AuthModel.AuthStatus status = _status as AuthModel.AuthStatus;
            var service = findService(AuthService.NAME) as AuthService;
            Proto.Group.ElementWhereRequest request = new Proto.Group.ElementWhereRequest();
            request.key = status.uuid;
            service.PostGroupElementWhere(request);
        }

        private void handleAuthSuccess(Model.Status _status, object _data)
        {
            var rsp = _data as Dictionary<string, Any>;
            var bridge = myFacade.getUiBridge() as IStartupUiBridge;
            bridge.HandleSignin(rsp["code"].AsInt32(), rsp["message"].AsString());
        }


        private void handleAuthFailure(Model.Status _status, object _data)
        {
            var rsp = _data as Dictionary<string, Any>;
            var bridge = myFacade.getUiBridge() as IStartupUiBridge;
            bridge.HandleSignin(rsp["code"].AsInt32(), rsp["message"].AsString());
        }

        private void handleAuthWhere(Model.Status _status, object _data)
        {
            AuthModel.AuthStatus status = _status as AuthModel.AuthStatus;
            var rsp = _data as Dictionary<string, Any>;
            var service = findService(AuthService.NAME) as AuthService;
            Proto.Group.ElementGetRequest request = new Proto.Group.ElementGetRequest();

            if (string.IsNullOrEmpty(status.activeElement.Key))
            {

                rsp["code"] = Any.FromInt32(12);
                rsp["message"] = Any.FromString("element not found");
                var bridge = myFacade.getUiBridge() as IStartupUiBridge;
                bridge.HandleSignin(rsp["code"].AsInt32(), rsp["message"].AsString());
                return;
            }
            request.uuid = status.activeElement.Value;
            service.PostGroupElementGet(request);
        }

        private void handleAuthRole(Model.Status _status, object _data)
        {
            AuthModel.AuthStatus status = _status as AuthModel.AuthStatus;
            var rsp = _data as Dictionary<string, Any>;
            var service = findService(AuthService.NAME) as AuthService;
            Proto.Permission.ScopeSearchRequest request = new Proto.Permission.ScopeSearchRequest();
            request.offset = 0;
            request.count = int.MaxValue;
            request.key = string.Format("{0}.{1}", status.activeElement.Key, status.role);
            service.PostPermissionScopeSearch(request);
        }

        private void handleAuthPermission(Model.Status _status, object _data)
        {
            AuthModel.AuthStatus status = _status as AuthModel.AuthStatus;
            var rsp = _data as Dictionary<string, Any>;
            var service = findService(AuthService.NAME) as AuthService;
            Proto.Permission.RuleListRequest request = new Proto.Permission.RuleListRequest();
            request.offset = 0;
            request.count = int.MaxValue;
            request.scope = status.permissionScopeUUID;
            service.PostPermissionRuleList(request);
        }


    }
}
