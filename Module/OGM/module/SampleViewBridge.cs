using XTC.oelMVCS;
namespace OGM.Module.Sample
{
    public class SampleViewBridge : ISampleViewBridge
    {
        public SampleView view { get; set; }
        public void onLoginSubmit(string _username, string _password)
        {
            view.onLoginSubmit(_username, _password);
        }
    }
}