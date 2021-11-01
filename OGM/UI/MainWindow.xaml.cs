using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XTC.oelMVCS;

namespace OGM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public class MainWindowUiBridge : IMainWindowUiBridge
        {
            public MainWindow control { get; set; }

            public void Alert(string _message)
            {
                HandyControl.Controls.Growl.Warning(_message, "StatusGrowl");
            }

            public void CloseTaskPanel()
            {
                control.drawerTask.IsOpen = false;
            }

            public void OpenTaskPanel()
            {
                control.drawerTask.IsOpen = true;
            }

            public void RefreshTaskList(List<Dictionary<string, string>> _tasks)
            {
                bool wait = false;
                control.lbTask.Items.Clear();
                foreach (var e in _tasks)
                {
                    control.lbTask.Items.Add(e["id"]);
                    if (e["wait"].Equals("True"))
                        wait = true;
                }
                control.badgeTask.ShowBadge = _tasks.Count > 0;
                control.badgeTask.Value = _tasks.Count;
                if (_tasks.Count == 0)
                    control.drawerTask.IsOpen = false;
                control.drawerTask.MaskCanClose = !wait;
            }
        }
        private List<Paragraph> cache_ = new List<Paragraph>();
        private MainWindowFacade facade_ = null;
        public MainWindow()
        {
            InitializeComponent();

            facade_ = FacadeCache.facadeMainWindow;
            MainWindowUiBridge bridge = new MainWindowUiBridge();
            bridge.control = this;
            facade_.setUiBridge(bridge);

            badgeTask.ShowBadge = false;
            badgeLogger.Value = 0;
        }

        public void OnLoggerAppended(string _text, Color _color)
        {
            Paragraph p = new Paragraph(new Run(_text));
            p.Foreground = new SolidColorBrush(_color);
            rtbLogger.Document.Blocks.Add(p);
            badgeLogger.ShowBadge = rtbLogger.Document.Blocks.Count > 0;
            badgeLogger.Value = rtbLogger.Document.Blocks.Count;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState.Maximized == WindowState)
                return;
            DragMove();
        }

        private void onLoggerClicked(object sender, RoutedEventArgs e)
        {
            drawerBottom.IsOpen = true;
        }

        private void onTaskClicked(object sender, RoutedEventArgs e)
        {
            drawerTask.IsOpen = true;
        }

        private void OnStateChanghed(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    this.MaxWidth = SystemParameters.WorkArea.Width + 16;
                    this.MaxHeight = SystemParameters.WorkArea.Height + 16;
                    break;
                case WindowState.Normal:
                    break;
            }

        }

    }
}
