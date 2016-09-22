﻿using Jandan.UWP.Core.ViewModels;
using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FreshPage : Page
    {
        FreshViewModel _viewModel;

        public FreshPage()
        {
            this.InitializeComponent();

            DataContext = _viewModel = new FreshViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.FreshPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Update();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void FreshGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(FreshDetailPage), new object[] { 0, e.ClickedItem as Core.Models.Fresh });
        }

        private void FreshPullToRefresh_RefreshInvoked(DependencyObject sender, object args)
        {
            _viewModel.Update();
        }

        private void PageTitleIcon_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            
        }
    }
}
