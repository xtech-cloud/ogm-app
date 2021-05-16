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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OGM
{
    /// <summary>
    /// Titlebar.xaml 的交互逻辑
    /// </summary>
    public partial class Titlebar : UserControl
    {
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

        public Titlebar()
        {
            InitializeComponent();

            MaximumVisibility = Visibility.Visible;
            WindowVisibility = Visibility.Hidden;
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
    }
}
