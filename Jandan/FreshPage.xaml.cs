﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Jandan.UWP.Models;
using Jandan.UWP.ViewModels;
using Jandan.UWP.Tools;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FreshPage : Page
    {
        FreshViewModel _viewModel;

        public FreshPage()
        {
            this.InitializeComponent();

            DispatcherManager.Current.Dispatcher = Dispatcher;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.FreshPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
            this.DataContext = _viewModel = new FreshViewModel();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void FreshListView_ItemClick(object sender, ItemClickEventArgs e)
        {            
            this.Frame.Navigate(typeof(FreshDetailPage), new object[] { e.ClickedItem as Fresh });
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}
