using System.Collections.Generic;
using XTC.oelMVCS;

namespace OGM
{
    public class AuthModel : Model
    {
        public const string NAME = "AuthModel";

        public class AuthStatus : Model.Status
        {
            public const string NAME = "AuthStatus";
            public string accessToken { get; set; }
            public Dictionary<string, string> element { get; set; }
            public KeyValuePair<string, string> activeElement { get; set; }
            public string uuid { get; set; }
            public string role { get; set; }

            public string permissionScopeUUID { get; set; }
            public Dictionary<string, string> permission = new Dictionary<string, string>();
            public AuthStatus()
            {
                accessToken = "";
                uuid = "";
                element = new Dictionary<string, string>();
                activeElement = new KeyValuePair<string, string>("", "");
            }
        }


        protected override void preSetup()
        {
            Error err;
            status_ = spawnStatus<AuthStatus>(AuthStatus.NAME, out err);
            if (0 != err.getCode())
            {
                getLogger().Error(err.getMessage());
            }
        }

        protected override void setup()
        {
            getLogger().Trace("setup AuthModel");
        }

        protected override void preDismantle()
        {
            Error err;
            killStatus(AuthStatus.NAME, out err);
            if (0 != err.getCode())
            {
                getLogger().Error(err.getMessage());
            }
        }

        private AuthStatus status
        {
            get
            {
                return status_ as AuthStatus;
            }
        }

        public void UpdateSigninReply(Model.Status _reply, string _accessToken, string _uuid)
        {
            Dictionary<string, Any> data = new Dictionary<string, Any>();
            data["code"] = Any.FromInt32(_reply.getCode());
            data["message"] = Any.FromString(_reply.getMessage());
            if (_reply.getCode() != 0)
            {
                Broadcast("/Application/Auth/Failure", data);
                return;
            }

            status.accessToken = _accessToken;
            status.uuid = _uuid;

            data["accessToken"] = Any.FromString(_accessToken);
            data["uuid"] = Any.FromString(_uuid);
            Broadcast("/Application/Auth/Signin", data);
            //Broadcast("/Application/Auth/Success", data);
        }

        public void UpdateElementWhereReply(Model.Status _reply, Dictionary<string, string> _element)
        {
            Dictionary<string, Any> data = new Dictionary<string, Any>();
            data["code"] = Any.FromInt32(_reply.getCode());
            data["message"] = Any.FromString(_reply.getMessage());
            if (_reply.getCode() != 0)
            {
                Broadcast("/Application/Auth/Failure", data);
                return;
            }

            if (_element.Count == 0)
            {
                data["code"] = Any.FromInt32(10);
                data["message"] = Any.FromString("group not found");
                Broadcast("/Application/Auth/Failure", data);
                return;
            }

            status.element = _element;
            foreach (var pair in _element)
            {
                status.activeElement = pair;
            }
            Broadcast("/Application/Auth/Where", data);
        }
        public void UpdateElementGetReply(Model.Status _reply, string[] _label)
        {
            Dictionary<string, Any> data = new Dictionary<string, Any>();
            data["code"] = Any.FromInt32(_reply.getCode());
            data["message"] = Any.FromString(_reply.getMessage());
            if (_reply.getCode() != 0)
            {
                Broadcast("/Application/Auth/Failure", data);
                return;
            }

            foreach (var label in _label)
            {
                if (label.StartsWith("role."))
                    status.role = label.Remove(0, "role.".Length);
            }

            Broadcast("/Application/Auth/Role", data);
        }

        public void UpdatePermissionScopeSearchReply(Model.Status _reply, string _uuid)
        {
            Dictionary<string, Any> data = new Dictionary<string, Any>();
            data["code"] = Any.FromInt32(_reply.getCode());
            data["message"] = Any.FromString(_reply.getMessage());
            if (_reply.getCode() != 0)
            {
                Broadcast("/Application/Auth/Failure", data);
                return;
            }
            if (string.IsNullOrEmpty(_uuid))
            {
                data["code"] = Any.FromInt32(10);
                data["message"] = Any.FromString("scope of permission not found");
                Broadcast("/Application/Auth/Failure", data);
                return;
            }
            status.permissionScopeUUID = _uuid;
            Broadcast("/Application/Auth/Permission", data);
        }

        public void UpdateRuleListReply(Model.Status _reply, Proto.Permission.RuleEntity[] _entity)
        {
            Dictionary<string, Any> data = new Dictionary<string, Any>();
            data["code"] = Any.FromInt32(_reply.getCode());
            data["message"] = Any.FromString(_reply.getMessage());
            if (_reply.getCode() != 0)
            {
                Broadcast("/Application/Auth/Failure", data);
                return;
            }

            status.permission.Clear();
            foreach(var entity in _entity)
            {
                if (1 != entity.state)
                    continue;
                status.permission[entity.key] = entity.name;

            }

            Broadcast("/Application/Auth/Success", data);
            Broadcast("/permission/updated", status.permission);
        }


    }
}
