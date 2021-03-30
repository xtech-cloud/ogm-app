using XTC.oelMVCS;

namespace app
{
    class AppFacade : View.Facade
    {
        public const string NAME = "AppFacade";

        public RootForm rootForm { get; set; }

        public void AttachPanel(object _panel)
        {
            rootForm.SetMainPanel(_panel);
        }
    }
}
