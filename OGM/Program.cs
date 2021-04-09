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
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ModuleManager moduleMgr = new ModuleManager();

            AppForm appForm = new AppForm();
            StartupForm startupForm = new StartupForm();
            startupForm.appForm = appForm;

            ConsoleLogger logger = new ConsoleLogger();
            logger.rtbLog = appForm.getLoggerUi();
            Config config = new AppConfig();
            startupForm.config = config;

            Framework framework = new Framework();
            framework.setLogger(logger);
            framework.setConfig(config);
            framework.Initialize();

            AppFacade appFacade = new AppFacade();
            appFacade.form = appForm;
            framework.getStaticPipe().RegisterFacade(AppFacade.NAME, appFacade);
            AppView appView = new AppView();
            appView.moduleMgr = moduleMgr;
            framework.getStaticPipe().RegisterView(AppView.NAME, appView);

            moduleMgr.logger = logger;
            moduleMgr.framework = framework;
            // 注册模块
            moduleMgr.Register();

            framework.Setup();

            startupForm.Show();
            appForm.Hide();
            Application.Run();

            // 注销模块窗体
            moduleMgr.Cancel();


            framework.Dismantle();
            framework.Release();
        }
    }
}
