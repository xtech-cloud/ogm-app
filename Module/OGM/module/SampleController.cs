using System;
using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    public class SampleController: Controller
    {
        public const string NAME = "Sample.SampleController";
        private SampleView view = null;

        protected override void preSetup()
        {
            view = findView(SampleView.NAME) as SampleView;
        }


        protected override void setup()
        {
            getLogger().Trace("setup ModuleController");
        }

        public void ProcessLoginResult(SampleModel.SampleStatus _status)
        {
            if(0 == _status.getCode())
            {
                view.refreshTip(string.Format("userid:{0}", _status.userid));
            }
            else
            {
                view.refreshTip(_status.getMessage());
            }
        }
    }
}
