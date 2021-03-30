using XTC.oelMVCS;
namespace OGM.Module.Sample
{
    public interface ISampleUiBridge : View.Facade.Bridge
    {
        object getRootPanel();
        void refreshErrorTip(string _tip);
    }
}
