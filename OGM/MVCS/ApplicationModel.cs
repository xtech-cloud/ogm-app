using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class ApplicationModel : Model
    {
        public const string NAME = "ApplicationModel";

        public class Task
        {
            public string id { get; set; }
            public int value { get; set; }
            public bool wait { get; set; }
        }

        private ApplicationStatus status
        {
            get
            {
                return status_ as ApplicationStatus;
            }
        }

        public class ApplicationStatus : Status
        {
            public const string NAME = "ApplicationStatus";
            public List<Task> task = new List<Task>();
        }

        protected override void preSetup()
        {
            Error err;
            status_ = spawnStatus<ApplicationStatus>(ApplicationStatus.NAME, out err);
        }

        protected override void setup()
        {
        }

        protected override void postDismantle()
        {
            Error err;
            killStatus(ApplicationStatus.NAME, out err);
        }

        public void SaveAddTask(string _id, bool _wait)
        {
            Task task = new Task();
            task.id = _id;
            task.wait = _wait;
            status.task.Add(task);
        }

        public void SaveRemoveTask(string _id)
        {
            status.task.RemoveAll((_item) =>
            {
                return _item.id.Equals(_id);
            });
        }

        public void SaveUpdateTask(string _id, int _value)
        {
            var found = status.task.Find((_item) =>
            {
                return _item.id.Equals(_id);
            });
            if (null == found)
                return;
            found.value = _value;
        }
    }
}
