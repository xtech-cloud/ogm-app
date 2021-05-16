using XTC.oelMVCS;

namespace OGM
{
    public interface IMainContentViewBridge : View.Facade.Bridge
    {
    }

    public interface IMainContentUiBridge : View.Facade.Bridge
    {
        void SwitchPage(string _page);
    }
}
