using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class MainWindowView : View
    {

        public class MainWindowViewBridge : IMainWindowViewBridge
        {
            public MainWindowView view { get; set; }
        }

        public const string NAME = "MainWindowView";

        private Facade myFacade = null;
        private ApplicationModel model = null;

        protected override void preSetup()
        {
            model = findModel(ApplicationModel.NAME) as ApplicationModel;
            myFacade = findFacade(MainWindowFacade.NAME);
            MainWindowViewBridge vb = new MainWindowViewBridge();
            vb.view = this;
            myFacade.setViewBridge(vb);
        }

        protected override void postSetup()
        {
            addObserver(ApplicationModel.NAME, "/Task/Begin", this.handleTaskBegin);
            addObserver(ApplicationModel.NAME, "/Task/End", this.handleTaskEnd);
            addObserver(ApplicationModel.NAME, "/Task/Update", this.handleTaskUpdate);
        }

        private void handleTaskBegin(Model.Status _status, object _data)
        {
            // 在数据层中添加任务
            Dictionary<string, Any> data = (Dictionary<string, Any>)_data;
            string id = data["id"].AsString();
            bool wait = data["wait"].AsBool();
            model.SaveAddTask(id, wait);

            refreshTask(_status);
        }

        private void handleTaskEnd(Model.Status _status, object _data)
        {
            // 在数据层中移除任务
            Dictionary<string, Any> data = (Dictionary<string, Any>)_data;
            string id = data["id"].AsString();
            model.SaveRemoveTask(id);

            refreshTask(_status);
        }

        private void handleTaskUpdate(Model.Status _status, object _data)
        {
            // 在数据层中更新任务
            Dictionary<string, Any> data = (Dictionary<string, Any>)_data;
            string id = data["id"].AsString();
            int value = data["value"].AsInt32();
            model.SaveUpdateTask(id, value);

            refreshTask(_status);
        }

        private void refreshTask(Model.Status _status)
        {
            ApplicationModel.ApplicationStatus status = _status.Access(ApplicationModel.ApplicationStatus.NAME) as ApplicationModel.ApplicationStatus;
            IMainWindowUiBridge bridge = myFacade.getUiBridge() as IMainWindowUiBridge;
            List<Dictionary<string, string>> task = new List<Dictionary<string, string>>();
            foreach (var e in status.task)
            {
                var dict = new Dictionary<string, string>();
                dict["id"] = e.id;
                dict["value"] = e.value.ToString();
                dict["wait"] = e.wait.ToString();
                task.Add(dict);
            }
            if (task.Count > 0)
                bridge.OpenTaskPanel();
            else
                bridge.CloseTaskPanel();
            bridge.RefreshTaskList(task);
        }
    }
}
