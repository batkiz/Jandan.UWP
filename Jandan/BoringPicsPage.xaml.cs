﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BoringPicsPage : Page
    {
        BoringViewModel _viewModel;
        DuanCommentViewModel _dViewModel;

        private int secret_count;

        public BoringPicsPage()
        {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.BoringPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
            this.DataContext = _viewModel = new BoringViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();

            secret_count = 0;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void BoringListView_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    DuanSplitView.IsPaneOpen = true;

        //    var d = e.ClickedItem as BoringPic;
        //    var commentId = d.PicID;

        //    _dViewModel.Update(commentId);
        //}

        private void BoringListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic });
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic });
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = e.OriginalSource as Button;
            var r = b.Parent as RelativePanel;
            var g = r.Parent as Grid;
            var c = g.Children[0] as RelativePanel;
            var t = c.Children[1] as TextBlock;
            var id = t.Text;

            _dViewModel.Update(id);

            DuanSplitView.IsPaneOpen = true;
        }

        private void PageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            secret_count++;

            if (secret_count == 5)
            {
                this.Frame.Navigate(typeof(MeiziPicsPage));
                secret_count = 0;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}
