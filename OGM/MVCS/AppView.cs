using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class AppView : View
    {
        public const string NAME = "AppView";

        public ModuleManager moduleMgr { get; set; }

        private AppFacade appFacade = null;

        protected override void preSetup()
        {
            appFacade = findFacade(AppFacade.NAME) as AppFacade;
        }

        protected override void setup()
        {
            route("/module/view/attach", this.handleAttachView);
        }


        protected override void dismantle()
        {
        }

        private void handleAttachView(Model.Status _status, object _data)
        {
            getLogger().Trace("attach view");
            Dictionary<string, object> data = _data as Dictionary<string, object>;
            foreach (string uri in data.Keys)
            {
                string path = moduleMgr.convertPath(uri);
                appFacade.form.AddPath(path, data[uri]);
            }
        }
    }
}
