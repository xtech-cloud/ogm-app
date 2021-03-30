using System;
using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    public class SampleView: View
    {
        public const string NAME = "Sample.SampleView";

        private Facade facade = null;
        private SampleModel model = null;
        private SampleService service = null;
        private ISampleUiBridge bridge = null;

        protected override void preSetup()
        {
            service = findService(SampleService.NAME) as SampleService;
            model = findModel(SampleModel.NAME) as SampleModel;
            facade = findFacade("Sample.SampleFacade");
            SampleViewBridge vb = new SampleViewBridge();
            vb.view = this;
            facade.setViewBridge(vb);
        }

        protected override void setup()
        {
            getLogger().Trace("setup ModuleView");
        }

        protected override void postSetup()
        {
            bridge = facade.getUiBridge() as ISampleUiBridge;
            object rootPanel = bridge.getRootPanel();
            // 通知主程序挂载界面
            model.Broadcast("/module/view/attach", rootPanel);
        }

        public void onLoginSubmit(string _username, string _password)
        {
            if (string.IsNullOrEmpty(_username))
            {
                refreshTip("username is empty");
                return;
            }

            if (string.IsNullOrEmpty(_password))
            {
                refreshTip("password is empty");
                return;
            }

            service.PostLogin(_username, _password);
        }

        public void refreshTip(string _tip)
        {
            bridge.refreshErrorTip(_tip);
        }
    }
}
