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
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DuanPage : Page
    {
        DuanViewModel _viewModel;
        DuanCommentViewModel _dViewModel;

        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        public DuanPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new DuanViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_persistedPosition))
            {
                await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(this.DuanListView, _persistedPosition, this.GetItem);
            }
        }

        private IAsyncOperation<object> GetItem(string key)
        {
            if (_viewModel.Duans == null)
            {
                return null;
            }
            return Task.Run(() =>
            {
                if (_viewModel.Duans.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return (object)_viewModel.Duans.FirstOrDefault(i => i.DuanID == key);
                }
            }).AsAsyncOperation();
        }

        private string GetKey(object item)
        {
            var singleItem = item as Duan;
            if (singleItem != null)
            {
                _persistedItemContainerHeight = (DuanListView.ContainerFromItem(item) as ListViewItem).ActualHeight;
                _persistedItemKey = singleItem.DuanID;
                return _persistedItemKey;
            }
            else
            {
                return string.Empty;
            }
        }

        private void DuanListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var singleItem = args.Item as Duan;

            if (singleItem != null && singleItem.DuanID == _persistedItemKey)
            {
                if (!args.InRecycleQueue)
                {
                    args.ItemContainer.Height = _persistedItemContainerHeight;
                }
                else
                {
                    args.ItemContainer.ClearValue(HeightProperty);
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _persistedPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(DuanListView, GetKey);

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.DuanPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
            //this.DataContext = _viewModel = new DuanViewModel();
            //DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
        }

        private void DuanListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DuanSplitView.IsPaneOpen = true;
            
            var d = e.ClickedItem as Duan;
            var commentId = d.DuanID;

            _dViewModel.Update(commentId);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void pullToRefreshBar_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }

        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            _dViewModel.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void DuanSplitView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DuanGridView.Visibility == Visibility.Collapsed)
            {
                return;
            }

            double margin = DuanGridView.Padding.Left + DuanGridView.Padding.Right;

            double currentWidth = this.ActualWidth - margin;
            int columnCount = (int)Math.Floor(currentWidth / 230);
            double setWidth = (currentWidth - (columnCount - 1) * 10) / columnCount;

            var s = DuanGridView.ItemContainerStyle;
            Setter s4 = new Setter(GridViewItem.WidthProperty, setWidth);
            Setter s3 = new Setter(GridViewItem.VerticalContentAlignmentProperty, VerticalAlignment.Top);

            Style s_new = new Style(typeof(GridViewItem));
            s_new.Setters.Add(s3);
            s_new.Setters.Add(s4);

            DuanGridView.ItemContainerStyle = s_new;
        }

        private async void DuanVotePositiveIcon_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            var msg = await _viewModel.Vote(duan.DuanID, true);
            duan.VotePositive = duan.VotePositive + 1;
            b.DataContext = duan;
        }

        private async void DuanVoteNegativeIcon_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

           var msg = await _viewModel.Vote(duan.DuanID, false);
            duan.VoteNegative = duan.VoteNegative + 1;
            b.DataContext = duan;
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ShowFlyout(sender);
        }

        private static void ShowFlyout(object sender)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowFlyout(sender);
        }

        private void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            string copied_content = $"转自煎蛋网:\n作者:{d.Author}\nID:{d.DuanID}\n{d.Content}\n(jandan.net | 地球没有新鲜事)";
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(copied_content);
            Clipboard.SetContent(dataPackage);
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //double delta = e.Cumulative.Translation.X;

            //if (delta > 200)
            //{
            //    Frame.Navigate(typeof(FreshPage));
            //}
            //else if (delta < 200)
            //{
            //    Frame.Navigate(typeof(BoringPicsPage));
            //}
        }


    }
}
