using XTC.oelMVCS;

namespace OGM
{
    public class AppView : View
    {
        public const string NAME = "AppView";
        private View.UiFacade appFacade = null;

        protected override void preSetup()
        {
            //appFacade.setView(this);
        }

        protected override void setup()
        {
        }


        protected override void dismantle()
        {
        }

        public void onLogCheckedChanged(bool _checked)
        {

        }
    }
}
