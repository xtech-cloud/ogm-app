using XTC.oelMVCS;
using System.Collections.Generic;

namespace OGM
{
    public interface IMainWindowViewBridge : View.Facade.Bridge
    {
    }

    public interface IMainWindowUiBridge : View.Facade.Bridge
    {
        void Alert(string _message);

        void OpenTaskPanel();
        void CloseTaskPanel();
        void RefreshTaskList(List<Dictionary<string, string>> _tasks);
    }
}
