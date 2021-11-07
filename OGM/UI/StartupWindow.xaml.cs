using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OGM
{
    /// <summary>
    /// StartupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartupWindow
    {
        public Action<string> OnAuthSuccess { get; set; }
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void onSigninClick(object sender, RoutedEventArgs e)
        {
            OnAuthSuccess("");
        }
    }
}
