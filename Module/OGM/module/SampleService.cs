using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    public class SampleService: Service
    {
        public const string NAME = "Sample.SampleService";
        private SampleModel model = null;

        protected override void preSetup()
        {
            model = findModel(SampleModel.NAME) as SampleModel;
        }
        protected override void setup()
        {
            getLogger().Trace("setup ModuleService");
        }

        public void PostLogin(string _username, string _password)
        {
            Dictionary<string, Any> paramMap = new Dictionary<string, Any>();
            paramMap["username"] = Any.FromString(_username);
            paramMap["password"] = Any.FromString(_password);
            post("/login", paramMap, (_reply) =>
            {
                SampleModel.SampleStatus status = new SampleModel.SampleStatus();
                status.userid = "121212";
                model.UpdateLoginResult(status);
            }, (_err) =>
             {
                 SampleModel.SampleStatus status = Model.Status.New<SampleModel.SampleStatus>(_err.getCode(), _err.getMessage());
                 model.UpdateLoginResult(status);
             }, null);
        }
    }
}
