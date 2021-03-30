using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OGM.Module.Sample;
using OGM.Module.Sample.WinForm;

namespace app
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RootForm rootForm = new RootForm();

            ModuleRoot moduleRoot = new ModuleRoot();
            RootPanel rootPanel = new RootPanel();
            rootForm.SetMainPanel(rootPanel);

            Application.Run(rootForm);
        }
    }
}
