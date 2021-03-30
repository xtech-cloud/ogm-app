using XTC.oelMVCS;
namespace OGM.Module.Sample
{
    public interface ISampleViewBridge : View.Facade.Bridge
    {
        void onLoginSubmit(string _username, string _password);
    }
}
