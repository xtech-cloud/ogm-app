using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class TitlebarView : View
    {
        public class TitlebarViewBridge : ITitlebarViewBridge
        {
            public TitlebarView view { get; set; }

        }

        public const string NAME = "TitlebarView";

        private Facade myFacade = null;

        protected override void preSetup()
        {
            myFacade = findFacade(TitlebarFacade.NAME);
            TitlebarViewBridge vb = new TitlebarViewBridge();
            vb.view = this;
            myFacade.setViewBridge(vb);
        }

        protected override void setup()
        {
        }

        protected override void postSetup()
        {
        }
    }
}
