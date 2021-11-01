using XTC.oelMVCS;
using System.Collections.Generic;

namespace OGM
{
    public interface ITitlebarViewBridge : View.Facade.Bridge
    {
        void OnPrivateSigninSubmit(string _host, string _username, string _password);
        void OnPublicSigninSubmit(string _username, string _password);

        void SetStorageValue(string _key, string _value);
        string GetStorageValue(string _key);
    }

    public interface ITitlebarUiBridge : View.Facade.Bridge
    {
        void Alert(string _message);
        void RefreshSigninSuccess(string _location);
    }
}
