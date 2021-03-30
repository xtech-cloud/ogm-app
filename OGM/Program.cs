using System;
using System.Windows.Forms;
using XTC.oelMVCS;

namespace OGM
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ModuleManager mgr = new ModuleManager();
            mgr.Parse();

            Logger logger = new ConsoleLogger();
            Config config = new Config();
            Framework framework = new Framework();
            framework.setConfig(config);
            framework.setLogger(logger);
            framework.Initialize();

            AppForm appForm = new AppForm();
            AppFacade appFacade = new AppFacade();
            appFacade.form = appForm;
            framework.getStaticPipe().RegisterFacade(AppFacade.NAME, appFacade);

            AppView appView = new AppView();
            framework.getStaticPipe().RegisterView(AppView.NAME, appView);

            framework.Setup();


            Application.Run(appForm);

            framework.Dismantle();
            framework.Release();
        }
    }
}
