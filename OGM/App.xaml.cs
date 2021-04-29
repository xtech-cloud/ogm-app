using XTC.oelMVCS;
using System.Windows;

namespace OGM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ModuleManager moduleMgr = new ModuleManager();

            ConsoleLogger logger = new ConsoleLogger();
            //logger.rtbLog = appForm.getLoggerUi();
            Config config = new AppConfig();
            //startupForm.config = config;

            Framework framework = new Framework();
            framework.setLogger(logger);
            framework.setConfig(config);
            framework.Initialize();

            AppFacade appFacade = new AppFacade();
            //appFacade.form = appForm;
            framework.getStaticPipe().RegisterFacade(AppFacade.NAME, appFacade);
            AppView appView = new AppView();
            appView.moduleMgr = moduleMgr;
            framework.getStaticPipe().RegisterView(AppView.NAME, appView);

            moduleMgr.logger = logger;
            moduleMgr.framework = framework;
            // 注册模块
            moduleMgr.Register();

            framework.Setup();

            MainWindow wnd = new MainWindow();
            this.MainWindow = wnd;
            wnd.Show();
        }

    }
}
