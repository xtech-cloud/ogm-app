using XTC.oelMVCS;
using System.Windows;
using System.Collections.Generic;
using ogm.account;
using System;
using System.IO;

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
        private Config config_ { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 加载模块
            moduleMgr_.Load();

            // 静态管线注册组件
            registerMVCS();
            registerFacade();

            moduleMgr_.logger = logger_;
            moduleMgr_.framework = framework_;
            // 注册模块
            moduleMgr_.Register();

            framework_.Setup();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            moduleMgr_ = new ModuleManager();

            logger_ = new ConsoleLogger();
            config_ = new AppConfig();
            ConfigSchema schema = new ConfigSchema();
            schema.application = "MeeXStudio";
            if (File.Exists("./domain.conf"))
            {
                schema.domain_public = File.ReadAllText("./domain.conf");
            }
            else
            {
                schema.domain_public = "https://api.meex.tech";
            }
            config_.Merge(System.Text.Json.JsonSerializer.Serialize(schema));

            logger_.setLevel(LogLevel.ALL);
            logger_.Info("OnStartup");
            framework_ = new Framework();
            framework_.setLogger(logger_);
            framework_.setConfig(config_);
            framework_.Initialize();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            framework_.Dismantle();
            moduleMgr_.Unload();
            moduleMgr_.Cancel();
            cancelFacade();
            cancelMVCS();
            framework_.Release();
            framework_ = null;
        }

        private void registerMVCS()
        {
            ApplicationModel applicationModel = new ApplicationModel();
            framework_.getStaticPipe().RegisterModel(ApplicationModel.NAME, applicationModel);

            BlankModel blankModel = new BlankModel();
            framework_.getStaticPipe().RegisterModel(BlankModel.NAME, blankModel);

            StorageModel storageModel = new StorageModel();
            framework_.getStaticPipe().RegisterModel(StorageModel.NAME, storageModel);
            storageModel.saveDelegate = saveStorage;
            storageModel.loadDelegate = loadStorage;

            framework_.getStaticPipe().RegisterModel(AuthModel.NAME, new AuthModel());
            framework_.getStaticPipe().RegisterService(AuthService.NAME, new AuthService());

            MainWindowView mainwindowView = new MainWindowView();
            framework_.getStaticPipe().RegisterView(MainWindowView.NAME, mainwindowView);

            TitlebarView titlebarView = new TitlebarView();
            framework_.getStaticPipe().RegisterView(TitlebarView.NAME, titlebarView);

            SideMenuView sideMenuView = new SideMenuView();
            framework_.getStaticPipe().RegisterView(SideMenuView.NAME, sideMenuView);

            MainContentView mainContentView = new MainContentView();
            mainContentView.moduleMgr = moduleMgr_;
            framework_.getStaticPipe().RegisterView(MainContentView.NAME, mainContentView);
        }

        private void registerFacade()
        {
            TitlebarFacade titlebarFacade = new TitlebarFacade();
            framework_.getStaticPipe().RegisterFacade(TitlebarFacade.NAME, titlebarFacade);
            FacadeCache.facadeTitlebar = titlebarFacade;
            Titlebar.TitlebarUiBridge bridgeBridge = new Titlebar.TitlebarUiBridge();
            titlebarFacade.setUiBridge(bridgeBridge);

            SideMenuFacade sideMenuFacade = new SideMenuFacade();
            framework_.getStaticPipe().RegisterFacade(SideMenuFacade.NAME, sideMenuFacade);
            FacadeCache.facadeSideMenu = sideMenuFacade;

            MainContentFacade mainContentFacade = new MainContentFacade();
            framework_.getStaticPipe().RegisterFacade(MainContentFacade.NAME, mainContentFacade);
            FacadeCache.facadeMainContent = mainContentFacade;

            MainWindowFacade facadeMainwindow = new MainWindowFacade();
            framework_.getStaticPipe().RegisterFacade(MainWindowFacade.NAME, facadeMainwindow);
            FacadeCache.facadeMainWindow = facadeMainwindow;
            MainWindow.MainWindowUiBridge bridgeMainWindow = new MainWindow.MainWindowUiBridge();
            titlebarFacade.setUiBridge(bridgeMainWindow);

            MainWindow wnd = new MainWindow();
            this.MainWindow = wnd;
            logger_.appendDelegate = wnd.OnLoggerAppended;
            wnd.Show();

            /*
            StartupWindow.AuthUiBridge bridge = new StartupWindow.AuthUiBridge();
            bridge.window = startupWindow;
            authFacade.setUiBridge(bridge);
            this.MainWindow = startupWindow;
            this.MainWindow.Height = 480;
            this.MainWindow.Width = 640;
            startupWindow.Show();
            startupWindow.onAuthSuccess = (_uuid) =>
            {
                MainWindow wnd = new MainWindow();
                this.MainWindow = wnd;
                logger_.rtbLogger = wnd.rtbLogger;
                wnd.Show();
                startupWindow.Close();
            };
            */
        }

        private void cancelMVCS()
        {
            framework_.getStaticPipe().CancelModel(StorageModel.NAME);
        }

        private void cancelFacade()
        {

        }

        private void saveStorage(ref Dictionary<string, Any> _data)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var pair in _data)
            {
                data[pair.Key] = pair.Value.AsString();
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "MeeXStudio");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, "storage.json");
            byte[] json_data = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data);
            File.WriteAllBytes(path, json_data);
        }

        private void loadStorage(ref Dictionary<string, Any> _data)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "MeeXStudio");
            path = Path.Combine(path, "storage.json");
            if (!File.Exists(path))
                return;
            byte[] json_data = File.ReadAllBytes(path);
            Dictionary<string, string> data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json_data);
            foreach (var pair in data)
            {
                _data[pair.Key] = Any.FromString(pair.Value);
            }

        }
    }
}
