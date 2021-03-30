using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM.Module.Sample
{
    class Mock
    {
        public static void Process(string _url, string _method, Dictionary<string, Any> _params, Service.OnReplyCallback _onReply, Service.OnErrorCallback _onError, Service.Options _options)
        {
            if(_params["username"].AsString().Equals("admin"))
            {
                _onReply("success");
            }
            else
            {
                _onError(Error.NewParamErr("username: admin"));
            }
        }
    }

    public class ModuleRoot
    {
        public ModuleRoot(Framework _framework)
        {
            framework_ = _framework;
        }

        public void Register()
        {
            // 注册数据层
            framework_.getStaticPipe().RegisterModel(SampleModel.NAME, new SampleModel());
            // 注册视图层
            framework_.getStaticPipe().RegisterView(SampleView.NAME, new SampleView());
            // 注册控制层
            framework_.getStaticPipe().RegisterController(SampleController.NAME, new SampleController());
            // 注册服务层
            SampleService service = new SampleService();
            framework_.getStaticPipe().RegisterService(SampleService.NAME, service);
            service.useMock = true;
            service.MockProcessor = Mock.Process;
        }

        public void Cancel()
        {
            // 注销数据层
            framework_.getStaticPipe().CancelModel(SampleModel.NAME);
            // 注销视图层
            framework_.getStaticPipe().CancelView(SampleView.NAME);
            // 注销控制层
            framework_.getStaticPipe().CancelController(SampleController.NAME);
            // 注销服务层
            framework_.getStaticPipe().CancelService(SampleService.NAME);
        }

        private Framework framework_ = null; 
    }
}
