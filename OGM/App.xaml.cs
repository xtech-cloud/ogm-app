using XTC.oelMVCS;
using System.Windows;

namespace OGM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Framework framework_ { get; set; }
        private ModuleManager moduleMgr_ { get; set; }
        private ConsoleLogger logger_ { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 静态管线注册组件
            registerMVCS();
            registerFacade();

            moduleMgr_.logger = logger_;
            moduleMgr_.framework = framework_;
            // 注册模块
            moduleMgr_.Register();

            framework_.Setup();

            MainWindow wnd = new MainWindow();
            this.MainWindow = wnd;
            wnd.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            moduleMgr_ = new ModuleManager();

            logger_ = new ConsoleLogger();
            //logger.rtbLog = appForm.getLoggerUi();
            Config config = new AppConfig();
            //startupForm.config = config;

            framework_ = new Framework();
            framework_.setLogger(logger_);
            framework_.setConfig(config);
            framework_.Initialize();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            moduleMgr_.Cancel();
            framework_.Release();
            framework_ = null;
        }

        private void registerMVCS()
        {
            BlankModel blankModel = new BlankModel();
            framework_.getStaticPipe().RegisterModel(BlankModel.NAME, blankModel);

            SideMenuView sideMenuView = new SideMenuView();
            framework_.getStaticPipe().RegisterView(SideMenuView.NAME, sideMenuView);

            MainContentView mainContentView = new MainContentView();
            mainContentView.moduleMgr = moduleMgr_;
            framework_.getStaticPipe().RegisterView(MainContentView.NAME, mainContentView);
        }

        private void registerFacade()
        {
            SideMenuFacade sideMenuFacade = new SideMenuFacade();
            framework_.getStaticPipe().RegisterFacade(SideMenuFacade.NAME, sideMenuFacade);
            App.Current.Resources.Add(SideMenuFacade.NAME, sideMenuFacade);

            MainContentFacade mainContentFacade = new MainContentFacade();
            framework_.getStaticPipe().RegisterFacade(MainContentFacade.NAME, mainContentFacade);
            App.Current.Resources.Add(MainContentFacade.NAME, mainContentFacade);
        }

    }
}
