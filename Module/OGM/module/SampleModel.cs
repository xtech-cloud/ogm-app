using System;
using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    public class SampleModel : Model
    {
        public const string NAME = "Sample.SampleModel";
        private SampleController controller = null;
        public class SampleStatus : Model.Status
        {
            public const string NAME = "SampleStatus";
            public string userid = "";
        }

        protected override void preSetup()
        {
            Error err;
            status_ = spawnStatus<SampleStatus>(SampleStatus.NAME, out err);
            controller = findController(SampleController.NAME) as SampleController;
        }

        protected override void setup()
        {
            getLogger().Trace("setup ModuleModel");
        }

        protected override void preDismantle()
        {
            Error err;
            killStatus(SampleStatus.NAME, out err);
        }

        public void UpdateLoginResult(SampleStatus _status)
        {
            status.userid = _status.userid;
            controller.ProcessLoginResult(_status);
        }

        private SampleStatus status
        {
            get
            {
                return status_ as SampleStatus;
            }
        }
    }
}
