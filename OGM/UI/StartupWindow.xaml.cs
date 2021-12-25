using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XTC.oelMVCS;

namespace OGM
{
    /// <summary>
    /// StartupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartupWindow
    {
        public class StartupUiBridge : IStartupUiBridge
        {
            public StartupWindow window { get; set; }

            public void HandleSettingLoad(Dictionary<string, Any> _settings)
            {
                Any domain;
                if (_settings.TryGetValue("domain", out domain))
                {
                    window.tbDomain.Text = domain.AsString();
                }

                Any apikey;
                if (_settings.TryGetValue("apikey", out apikey))
                {
                    window.tbApiKey.Text = apikey.AsString();
                }
            }

            public void HandleSignin(int _code, string _message)
            {
                if (0 == _code)
                {
                    window.OnAuthSuccess();
                    return;
                }

                window.tbError.Text = _message;
            }
        }

        public Action OnAuthSuccess;
        private StartupFacade facade_ { get; set; }

        public StartupWindow()
        {
            facade_ = FacadeCache.facadeStartup;
            StartupUiBridge bridge = new StartupUiBridge();
            bridge.window = this;
            facade_.setUiBridge(bridge);

            InitializeComponent();
            panelSetting.Visibility = Visibility.Hidden;
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
            tbError.Text = "";
            if (string.IsNullOrEmpty(tbUsername.Text) || string.IsNullOrEmpty(tbPassword.Password))
                return;
            var bridge = facade_.getViewBridge() as IStartupViewBridge;
            bridge.OnSigninSubmit(tbUsername.Text, wrapPassword(tbPassword.Password));
        }

        private void OnSettingClick(object sender, RoutedEventArgs e)
        {
            panelSetting.Visibility = Visibility.Visible;
            panelLogin.Visibility = Visibility.Hidden;
        }

        private void onCancelClick(object sender, RoutedEventArgs e)
        {
            panelSetting.Visibility = Visibility.Hidden;
            panelLogin.Visibility = Visibility.Visible;
        }

        private void onSaveClick(object sender, RoutedEventArgs e)
        {
            panelSetting.Visibility = Visibility.Hidden;
            panelLogin.Visibility = Visibility.Visible;
            var bridge = facade_.getViewBridge() as IStartupViewBridge;
            Dictionary<string, Any> settings = new Dictionary<string, Any>();
            settings["domain"] = Any.FromString(tbDomain.Text);
            settings["apikey"] = Any.FromString(tbApiKey.Text);
            bridge.OnSettingSubmit(settings);
        }

        private static string wrapPassword(string _password)
        {
            string password = reverseString(_password);
            password = toSHA512(password).ToUpper();
            password = reverseString(password);
            password = toMd5(password).ToUpper();
            return password;
        }

        private static string toMd5(string _value)
        {
            MD5 md5 = MD5.Create();
            byte[] byteOld = Encoding.UTF8.GetBytes(_value);
            byte[] byteNew = md5.ComputeHash(byteOld);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static string toSHA512(string _value)
        {
            SHA512 sha512 = new SHA512CryptoServiceProvider();
            byte[] byteOld = Encoding.UTF8.GetBytes(_value);
            byte[] byteNew = sha512.ComputeHash(byteOld);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }


        private static string reverseString(string _value)
        {
            char[] cs = _value.ToCharArray();
            Array.Reverse(cs);
            return new string(cs);
        }
    }
}
