﻿using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace OGM
{
    public partial class SideMenuControl
    {
        public class SideMenuUiBridge : ISideMenuUiBridge
        {
            public SideMenuControl control { get; set; }

            public void ActiveTab(string _name)
            {
                SideMenuItem tab;
                if (!control.tabs.TryGetValue(_name, out tab))
                    return;

                // 参考SideMenuItem源码实现tab切换
                var eventArgs = new RoutedEventArgs(SideMenuItem.SelectedEvent, tab);
                tab.RaiseEvent(eventArgs);
            }
        }

        private SideMenuFacade facade_ { get; set; }

        public SideMenuControl()
        {
            InitializeComponent();
            loadMenu();


            facade_ = FacadeCache.facadeSideMenu;
            SideMenuUiBridge bridge = new SideMenuUiBridge();
            bridge.control = this;
            facade_.setUiBridge(bridge);

        }

        private ICommand _switchItemCmd = null;
        public ICommand SwitchItemCmd
        {
            get
            {
                if (_switchItemCmd == null)
                {
                    _switchItemCmd = new RelayCommand(
                        this.SwitchItemCmd_CanExecute,
                        this.SwitchItemCmd_Execute);
                }
                return _switchItemCmd;
            }
        }

        private void SwitchItemCmd_Execute(FunctionEventArgs<object> _args)
        {
            SideMenuItem item = _args.Info as SideMenuItem;
            activeTab(item);
            _args.Handled = true;
        }
        public bool SwitchItemCmd_CanExecute(FunctionEventArgs<object> _args)
        {
            return true;
        }

        private void activeTab(SideMenuItem _item)
        {
            string entry = "";
            try
            {
                entry = (string)_item.FindResource("Entry");
            }
            catch (System.Exception ex)
            {
                // nothin to 
            }

            if (!string.IsNullOrEmpty(entry))
            {
                ISideMenuViewBridge bridge = facade_.getViewBridge() as ISideMenuViewBridge;
                bridge.OnTabActivated(entry);
            }

        }

        private Dictionary<string, SideMenuItem> tabs = new Dictionary<string, SideMenuItem>();

        private void loadMenu()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"./menu.xml");

            XmlNode xn = doc.SelectSingleNode("Menu");
            foreach (XmlNode node_group in xn.ChildNodes)
            {
                if (!node_group.Name.Equals("Group"))
                    continue;

                // title
                SideMenuItem itemTitle = new SideMenuItem();
                itemTitle.IsEnabled = false;
                itemTitle.FontSize = 14;
                itemTitle.Header = ((XmlElement)node_group).GetAttribute("Text");
                this.sidemenu.Items.Add(itemTitle);

                foreach (XmlNode node_section in node_group.ChildNodes)
                {
                    if (!node_section.Name.Equals("Section"))
                        continue;

                    // section
                    SideMenuItem itemSection = new SideMenuItem();
                    itemSection.Header = ((XmlElement)node_section).GetAttribute("Text");
                    System.Windows.Controls.Image iconSection = new System.Windows.Controls.Image();
                    iconSection.Width = 24;
                    iconSection.Height = 24;

                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(string.Format("pack://siteoforigin:,,,/{0}", ((XmlElement)node_section).GetAttribute("Icon")), UriKind.Absolute);
                        bitmap.EndInit();
                        //iconSection.Stretch = Stretch.Fill;
                        iconSection.Source = bitmap;
                    }
                    catch (System.Exception ex)
                    {

                    }
                    itemSection.Icon = iconSection;
                    this.sidemenu.Items.Add(itemSection);

                    foreach (XmlNode node_tab in node_section.ChildNodes)
                    {
                        if (!node_tab.Name.Equals("Tab"))
                            continue;

                        // tab
                        SideMenuItem itemTab = new SideMenuItem();
                        string entry = ((XmlElement)node_tab).GetAttribute("Entry");
                        itemTab.Resources.Add("Entry", entry);
                        itemTab.Padding = new System.Windows.Thickness(24, 0, 0, 0);
                        itemTab.Header = ((XmlElement)node_tab).GetAttribute("Text");
                        System.Windows.Controls.Image iconTab = new System.Windows.Controls.Image();
                        iconTab.Width = 24;
                        iconTab.Height = 24;
                        try
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(string.Format("pack://siteoforigin:,,,/{0}", ((XmlElement)node_tab).GetAttribute("Icon")), UriKind.Absolute);
                            bitmap.EndInit();
                            //iconSection.Stretch = Stretch.Fill;
                            iconTab.Source = bitmap;
                        }
                        catch (System.Exception ex)
                        {

                        }
                        itemTab.Icon = iconTab;
                        itemSection.Items.Add(itemTab);
                        tabs[entry] = itemTab;
                    }
                }
            }
        }
    }
}
