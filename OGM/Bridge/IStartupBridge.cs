using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public interface IStartupViewBridge : View.Facade.Bridge
    {
        void OnSigninSubmit(string _username, string _password);
        void OnSettingSubmit(Dictionary<string, Any> _settings);
    }

    public interface IStartupUiBridge : View.Facade.Bridge
    {
        void HandleSignin(int _code, string _message);
        void HandleSettingLoad(Dictionary<string, Any> _settings);
    }
}
