﻿using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using Jandan.UWP.Control;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan.UWP.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DuanPage : Page
    {
        /// <summary>
        /// 段子的View Model
        /// </summary>
        DuanViewModel _viewModel;

        // 用于从其他页面返回时保持滚动条的位置
        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        #region 基本功能
        public DuanPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new DuanViewModel();
            //DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
        }
        /// <summary>
        /// 从其他页面导航回到段子
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.DuanPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
        }

        #endregion

        #region 处理滚动条位置保存
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
        #endregion

        #region 刷新列表
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
        #endregion

        #region 段子评论
        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;
            
            CommentControl.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }
        #endregion
        
        #region OOXX功能
        private void DuanVotePositiveIcon_Click(object sender, RoutedEventArgs e)
        {
            DuanVote(sender, true);
        }

        private void DuanVoteNegativeIcon_Click(object sender, RoutedEventArgs e)
        {
            DuanVote(sender, false);
        }

        private async void DuanVote(object sender, bool isLike)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;
            var c = b.Parent as RelativePanel;

            var msg = await _viewModel.Vote(duan, true);

            if (msg == null)
            {
                await PopupMessage("网络不好，请稍后重试", 90, 2000);

                return;
            }

            if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    var t = c.Children[1] as TextBlock;
                    t.Text = (duan.VotePositive++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b1 = c.Children[0] as Button;
                    b1.Foreground = new SolidColorBrush(Colors.Red);
                    
                    await PopupMessage("感谢您的OO！", 64, 2000);
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (duan.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    await PopupMessage("感谢您的XX！", 64, 2000);
                }                
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                await PopupMessage("您已投过票了", 60, 2000);
            }
        }
        #endregion

        #region 段子复制
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

        private async void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            string copied_content = $"转自煎蛋网:\n作者:{d.Author}\nID:{d.DuanID}\n{d.Content}\n(jandan.net | 地球没有新鲜事)";
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(copied_content);
            Clipboard.SetContent(dataPackage);

            await PopupMessage("复制成功！", 40, 2000);
        }
        #endregion
                
        private async Task PopupMessage(string message, double textWidth, int disTime)
        {
            textBlockPopup.Text = message;
            popTipVote.HorizontalOffset = -textWidth;
            popTipVote.IsOpen = true;   // 提示再按一次
            await Task.Delay(disTime);  // 1000ms后关闭提示
            popTipVote.IsOpen = false;
        }        

        private void DuanSplitView_PaneClosed(SplitView sender, object args)
        {
            CommentControl.ClearResponse();            
        }
    }
}
