
using System;
using System.Windows;

namespace OGM
{
    /// <summary>
    ///     主内容
    /// </summary>
    public partial class MainContentControl
    {
        public class MainContentUiBridge : IMainContentUiBridge
        {
            public MainContentControl control { get; set; }

            public void SwitchPage(object _page)
            {
                control.SubContent = _page;
            }
        }

        public static readonly DependencyProperty SubContentProperty = DependencyProperty.Register("SubContent", typeof(object), typeof(MainContentControl));
        public object SubContent
        {
            get
            {
                return GetValue(MainContentControl.SubContentProperty);
            }
            set
            {
                SetValue(MainContentControl.SubContentProperty, value);
            }
        }
        

        private MainContentFacade facade_ { get; set; }

        public MainContentControl()
        {
            InitializeComponent();

            facade_ = FacadeCache.facadeMainContent;
            MainContentUiBridge bridge = new MainContentUiBridge();
            bridge.control = this;
            facade_.setUiBridge(bridge);
        }
    }
}
