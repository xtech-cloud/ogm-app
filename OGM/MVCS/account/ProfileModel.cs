
using System;
using XTC.oelMVCS;

namespace ogm.account
{
    public class ProfileModel : Model
    {
        public const string NAME = "ProfileModel";

        public class ProfileStatus : Model.Status
        {
            public const string NAME = "ProfileStatus";
        }


        protected override void preSetup()
        {
            Error err;
            status_ = spawnStatus<ProfileStatus>(ProfileStatus.NAME, out err);
            if(0 != err.getCode())
            {
                getLogger().Error(err.getMessage());
            }
        }

        protected override void setup()
        {
            getLogger().Trace("setup ogm.account.ProfileModel");
        }

        protected override void preDismantle()
        {
            Error err;
            killStatus(ProfileStatus.NAME, out err);
            if(0 != err.getCode())
            {
                getLogger().Error(err.getMessage());
            }
        }

        private ProfileStatus status
        {
            get
            {
                return status_ as ProfileStatus;
            }
        }
    }
}
