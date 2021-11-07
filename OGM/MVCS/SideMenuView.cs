using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class SideMenuView : View
    {
        public class SideMenuViewBridge : ISideMenuViewBridge
        {
            public SideMenuView view { get; set; }

            public void OnTabActivated(string _tab)
            {
                view.findModel(BlankModel.NAME).Broadcast(SideMenuAction.TAB_ACTIVATED, _tab);
            }
        }

        public const string NAME = "SideMenuView";

        private Facade myFacade = null;

        protected override void preSetup()
        {
            myFacade = findFacade(SideMenuFacade.NAME);
            SideMenuViewBridge vb = new SideMenuViewBridge();
            vb.view = this;
            myFacade.setViewBridge(vb);
        }
        protected override void postSetup()
        {
            addRouter(SideMenuAction.ACTIVE_TAB, this.handleActiveTab);
        }

        private void handleActiveTab(Model.Status _status, object _data)
        {
            string tab = (string)_data;
            var bridge = myFacade.getUiBridge() as ISideMenuUiBridge;
            bridge.ActiveTab((string)_data);
        }
    }
}
