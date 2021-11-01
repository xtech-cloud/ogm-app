using System;
using System.Collections.Generic;
using XTC.oelMVCS;

namespace ogm.account
{
    public class AuthModel : Model
    {
        public const string NAME = "AuthModel";

        public class AuthStatus : Model.Status
        {
            public class CloudAuth
            {
                public CloudAuth()
                {
                    accessToken = "";
                    uuid = "";
                }
                public string accessToken { get; set; }
                public string uuid { get; set; }
            }
            public const string NAME = "AuthStatus";
            public CloudAuth publicAuth { get; set; }
            public CloudAuth privateAuth { get; set; }
            public AuthStatus()
            {
                publicAuth = new CloudAuth();
                privateAuth = new CloudAuth();
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

        public void UpdateSigninReply(Model.Status _reply, string _accessToken, string _uuid, string _location, string _host)
        {
            Dictionary<string, Any> data = new Dictionary<string, Any>();
            data["code"] = Any.FromInt32(_reply.getCode());
            data["message"] = Any.FromString(_reply.getMessage());
            if (_reply.getCode() != 0)
            {
                Broadcast("/Application/Auth/Signin/Failure", data);
                return;
            }

            if (_location.Equals("Public"))
            {
                status.publicAuth.accessToken = _accessToken;
                status.publicAuth.uuid = _uuid;
            }
            else if (_location.Equals("Private"))
            {
                status.privateAuth.accessToken = _accessToken;
                status.privateAuth.uuid = _uuid;
            }

            data["accessToken"] = Any.FromString(_accessToken);
            data["uuid"] = Any.FromString(_uuid);
            data["location"] = Any.FromString(_location);
            data["host"] = Any.FromString(_host);
            Broadcast("/Application/Auth/Signin/Success", data);
        }
    }
}
