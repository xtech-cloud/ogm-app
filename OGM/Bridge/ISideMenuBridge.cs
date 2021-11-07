using XTC.oelMVCS;

namespace OGM
{
    public interface ISideMenuViewBridge : View.Facade.Bridge
    {
        void OnTabActivated(string _tab);
    }

    public interface ISideMenuUiBridge : View.Facade.Bridge
    {
        void ActiveTab(string _name);
    }
}
