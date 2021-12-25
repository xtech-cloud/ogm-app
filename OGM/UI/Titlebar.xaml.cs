using System.Windows;
using System.Windows.Controls;

namespace OGM
{
    /// <summary>
    /// Titlebar.xaml 的交互逻辑
    /// </summary>
    public partial class Titlebar : UserControl
    {

        public class TitlebarUiBridge : ITitlebarUiBridge
        {
            public Titlebar control { get; set; }

            public void Alert(string _message)
            {
                HandyControl.Controls.Growl.Warning(_message, "StatusGrowl");
            }

            public void RefreshSigninSuccess(string _location)
            {
            }
        }

        public static readonly DependencyProperty MaximumVisibilityProperty = DependencyProperty.Register("MaximumVisibility", typeof(Visibility), typeof(Titlebar));
        public static readonly DependencyProperty WindowVisibilityProperty = DependencyProperty.Register("WindowVisibility", typeof(Visibility), typeof(Titlebar));

        public Visibility MaximumVisibility
        {
            get { return (Visibility)GetValue(MaximumVisibilityProperty); }
            set { SetValue(MaximumVisibilityProperty, value); }
        }

        public Visibility WindowVisibility
        {
            get { return (Visibility)GetValue(WindowVisibilityProperty); }
            set { SetValue(WindowVisibilityProperty, value); }
        }

        private TitlebarFacade facade_ { get; set; }

        public Titlebar()
        {
            InitializeComponent();

            facade_ = FacadeCache.facadeTitlebar;
            TitlebarUiBridge bridge = new TitlebarUiBridge();
            bridge.control = this;
            facade_.setUiBridge(bridge);

            MaximumVisibility = Visibility.Hidden;
            WindowVisibility = Visibility.Visible;
        }

        private void OnMinimumClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void OnMaximumClick(object sender, RoutedEventArgs e)
        {
            MaximumVisibility = Visibility.Hidden;
            WindowVisibility = Visibility.Visible;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        private void OnWindowClick(object sender, RoutedEventArgs e)
        {
            MaximumVisibility = Visibility.Visible;
            WindowVisibility = Visibility.Hidden;
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void onProfileClicked(object sender, RoutedEventArgs e)
        {

        }

        private void onVersionClicked(object sender, RoutedEventArgs e)
        {
            openTopDrawer("version");
            tbVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void openTopDrawer(string _name)
        {
            drawerTop.IsOpen = true;
            spVersion.Visibility = _name.Equals("version") ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
