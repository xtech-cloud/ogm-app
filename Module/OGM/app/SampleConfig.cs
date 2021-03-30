using System;
using XTC.oelMVCS;

namespace app
{
    public class SampleConfig: Config
    {
        public override void Merge(string _content)
        {
            fields_["ui.facade"] = Any.FromString("winform"); 
        }
    }
}//namespace
