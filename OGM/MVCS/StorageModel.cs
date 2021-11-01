using XTC.oelMVCS;
using System.Collections.Generic;

namespace OGM
{
    public class StorageModel : Model
    {
        public const string NAME = "StorageModel";

        public delegate void SaveDelegate(ref Dictionary<string, Any> _pairs);
        public delegate void LoadDelegate(ref Dictionary<string, Any> _pairs);

        public SaveDelegate saveDelegate;
        public LoadDelegate loadDelegate;

        protected override void preSetup()
        {
            property_ = new Dictionary<string, Any>();
            isAllowSetProperty_ = true;
        }

        protected override void postSetup()
        {
            if(null != loadDelegate)
            {
                loadDelegate(ref property_);
            }
        }

        protected override void preDismantle()
        {
            if(null != saveDelegate)
            {
                saveDelegate(ref property_);
            }
        }
    }
}
