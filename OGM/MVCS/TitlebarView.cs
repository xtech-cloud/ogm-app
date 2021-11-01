using System.Collections.Generic;
using XTC.oelMVCS;
using ogm.account;

namespace OGM
{
    public class TitlebarView : View
    {
        public class TitlebarViewBridge : ITitlebarViewBridge
        {
            public TitlebarView view { get; set; }

            public void OnPrivateSigninSubmit(string _host, string _username, string _password)
            {
                ogm.account.Proto.SigninRequest req = new ogm.account.Proto.SigninRequest();
                req._strategy = Any.FromInt32(1);
                req._username = Any.FromString(_username);
                req._password = Any.FromString(_password);
                view.authService.PostSignin(req, "private", _host);
            }

            public void OnPublicSigninSubmit(string _username, string _password)
            {
                ogm.account.Proto.SigninRequest req = new ogm.account.Proto.SigninRequest();
                req._strategy = Any.FromInt32(1);
                req._username = Any.FromString(_username);
                req._password = Any.FromString(_password);
                view.authService.PostSignin(req, "public", view.getConfig().getField("domain.public").AsString());
            }

            public void SetStorageValue(string _key, string _value)
            {
                view.setStorageValue(_key, Any.FromString(_value));
            }

            public string GetStorageValue(string _value)
            {
                return view.getStorageValue(_value).AsString();
            }

        }

        public const string NAME = "TitlebarView";

        private Facade myFacade = null;
        private AuthService authService = null;

        protected override void preSetup()
        {
            authService = findService(AuthService.NAME) as AuthService;
            myFacade = findFacade(TitlebarFacade.NAME);
            TitlebarViewBridge vb = new TitlebarViewBridge();
            vb.view = this;
            myFacade.setViewBridge(vb);
        }

        protected override void setup()
        {
            addRouter("/Application/Auth/Signin/Success", handleAuthSigninSuccess);
            addRouter("/Application/Auth/Signin/Failure", handleAuthSigninFailure);
        }

        protected override void postSetup()
        {

        }


        private void handleAuthSigninSuccess(Model.Status _status, object _data)
        {
            ITitlebarUiBridge bridge = myFacade.getUiBridge() as ITitlebarUiBridge;
            Dictionary<string, Any> data = (Dictionary<string, Any>)_data;
            bridge.RefreshSigninSuccess(data["location"].AsString());
        }
        private void handleAuthSigninFailure(Model.Status _status, object _data)
        {
            ITitlebarUiBridge bridge = myFacade.getUiBridge() as ITitlebarUiBridge;
            Dictionary<string, Any> data = (Dictionary<string, Any>)_data;
            bridge.Alert(data["message"].AsString());
        }

        private void setStorageValue(string _key, Any _value)
        {
            var storageModel = findModel(StorageModel.NAME);
            storageModel.SetProperty(_key, _value);
        }

        private Any getStorageValue(string _key)
        {
            var storageModel = findModel(StorageModel.NAME);
            return storageModel.GetProperty(_key);
        }
    }
}
