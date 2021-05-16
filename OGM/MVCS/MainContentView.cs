﻿using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class MainContentView : View
    {
        public class MainContentViewBridge : IMainContentViewBridge
        {
            public MainContentView view { get; set; }
        }

        public const string NAME = "MainContentView";
        public ModuleManager moduleMgr { get; set; }

        private Facade myFacade = null;

        protected override void preSetup()
        {
            myFacade = findFacade(MainContentFacade.NAME);
            MainContentViewBridge vb = new MainContentViewBridge();
            vb.view = this;
            myFacade.setViewBridge(vb);
        }

        protected override void postSetup()
        {
            route("/module/view/attach", this.handleAttachView);
            route(SideMenuAction.TAB_ACTIVATED, handleTabActivated);
        }


        private void handleTabActivated(Model.Status _status, object _data)
        {
            string page = (string)_data;
            (myFacade.getUiBridge() as IMainContentUiBridge).SwitchPage(page);
        }

        private void handleAttachView(Model.Status _status, object _data)
        {
            getLogger().Trace("attach view");
            Dictionary<string, object> data = _data as Dictionary<string, object>;
            foreach (string uri in data.Keys)
            {
                string path = moduleMgr.convertPath(uri);
                //appFacade.form.AddPath(path, data[uri]);
            }
        }
    }
}
