using XTC.oelMVCS;
using System.Windows;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        private BlankModel blankModel_ { get; set; }

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

            updatePermission();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            moduleMgr_ = new ModuleManager();

            logger_ = new ConsoleLogger();
            config_ = new AppConfig();
            // 加载配置文件
            string curDir = System.IO.Directory.GetCurrentDirectory();
            string configsDir = Path.Combine(curDir, "configs");
            if (!Directory.Exists(configsDir))
                return;

            foreach (string entry in Directory.GetFiles(configsDir))
            {
                config_.Merge(File.ReadAllText(entry));
            }

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

            blankModel_ = new BlankModel();
            framework_.getStaticPipe().RegisterModel(BlankModel.NAME, blankModel_);

            StorageModel storageModel = new StorageModel();
            framework_.getStaticPipe().RegisterModel(StorageModel.NAME, storageModel);
            storageModel.saveDelegate = saveStorage;
            storageModel.loadDelegate = loadStorage;

            framework_.getStaticPipe().RegisterModel(AuthModel.NAME, new AuthModel());
            framework_.getStaticPipe().RegisterService(AuthService.NAME, new AuthService());

            StartupView startupView = new StartupView();
            framework_.getStaticPipe().RegisterView(StartupView.NAME, startupView);

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
            StartupFacade startupFacade = new StartupFacade();
            framework_.getStaticPipe().RegisterFacade(StartupFacade.NAME, startupFacade);
            FacadeCache.facadeStartup = startupFacade;

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


            StartupWindowFacade facadeStartupWindow = new StartupWindowFacade();
            framework_.getStaticPipe().RegisterFacade(StartupWindowFacade.NAME, facadeStartupWindow);
            StartupWindow startupWindow = new StartupWindow();
            FacadeCache.facadeMainWindow = facadeMainwindow;
            startupWindow.Show();
            startupWindow.OnAuthSuccess = () =>
            {
                MainWindow wnd = new MainWindow();
                this.MainWindow = wnd;
                logger_.appendDelegate = wnd.OnLoggerAppended;
                wnd.Show();

                //先最大化一次，避免出现窗口第一次最大化，任务栏消失的问题
                wnd.WindowState = WindowState.Maximized;
                wnd.WindowState = WindowState.Normal;
                wnd.WindowState = WindowState.Maximized;

                //将缓存的日志显示到UI上。
                logger_.Info("--------------------------------");
                startupWindow.Close();
            };
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

        private void updatePermission()
        {
            // 加载初始权限文件
            string curDir = System.IO.Directory.GetCurrentDirectory();
            string permissionsDir = Path.Combine(curDir, "permissions");
            if (!Directory.Exists(permissionsDir))
            {
                return;
            }
            Dictionary<string, string> permission = new Dictionary<string, string>();
            foreach (string entry in Directory.GetFiles(permissionsDir))
            {
                string json_data = File.ReadAllText(entry);
                Dictionary<string, string> dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json_data);
                foreach (var pair in dict)
                {
                    permission[pair.Key] = pair.Value;
                }
            }
            blankModel_.Broadcast("/permission/updated", permission);
        }
    }
}
