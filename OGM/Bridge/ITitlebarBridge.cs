using XTC.oelMVCS;
using System.Collections.Generic;

namespace OGM
{
    public interface ITitlebarViewBridge : View.Facade.Bridge
    {
    }

    public interface ITitlebarUiBridge : View.Facade.Bridge
    {
        void Alert(string _message);
        void RefreshSigninSuccess(string _location);
    }
}
