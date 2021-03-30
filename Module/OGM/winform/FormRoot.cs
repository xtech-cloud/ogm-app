using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    public class FormRoot
    {
        public FormRoot(Framework _framework)
        {
            framework_ = _framework;
        }

        public void Register()
        {
            // 注册UI装饰
            SampleFacade facade = new SampleFacade();
            framework_.getStaticPipe().RegisterFacade(SampleFacade.NAME, facade);

            SamplePanel panel = new SamplePanel();
            panel.facade = facade;
            SamplePanel.SampleUiBridge uiBridge = new SamplePanel.SampleUiBridge();
            uiBridge.panel = panel;
            facade.setUiBridge(uiBridge);
        }

        public void Cancel()
        {
            // 注销UI装饰
            framework_.getStaticPipe().CancelFacade(SampleFacade.NAME);
        }

        private Framework framework_ = null;
    }
}
