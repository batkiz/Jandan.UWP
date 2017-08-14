using Jandan.UWP.Core.Models;
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
using Jandan.UWP.Core.Tools;
using System.Collections.Generic;

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
        DuanViewModel ViewModel { get; } = new DuanViewModel();
        //PopupMessageControl _popupMsg = new PopupMessageControl();

        // 用于从其他页面返回时保持滚动条的位置
        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        #region 基本功能
        public DuanPage()
        {
            this.InitializeComponent();

            
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

            this.DataContext = ViewModel;

            if (DataShareManager.Current.CurrentPageIndex == DataShareManager.Current.PreviousPageIndex)
            {
                RefreshPage();
            }
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
            if (ViewModel.Duans == null)
            {
                return null;
            }
            return Task.Run(() =>
            {
                if (ViewModel.Duans.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return (object)ViewModel.Duans.FirstOrDefault(i => i.DuanID == key);
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
            ViewModel.Update();
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

            CommentControl.SetFocus();
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

            var msg = await ViewModel.Vote(duan, true);

            if (msg == null)
            {
                PopupMessage(2000, "网络不好，请稍后重试");

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

                    PopupMessage(2000, "感谢您的OO！");
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (duan.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    PopupMessage(2000, "感谢您的XX！");
                }                
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                PopupMessage(2000, "您已投过票了");
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
                
        private void MenuFlyoutCopy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            string copied_content = $"转自煎蛋网:\n作者:{d.Author}\nID:{d.DuanID}\n{d.Content}\n(jandan.net | 地球没有新鲜事)";
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(copied_content);
            Clipboard.SetContent(dataPackage);

            PopupMessage(2000, "复制成功！");
        }

        private async void MenuFlyoutFav_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            // 读取当前收藏列表
            var duan_list = await FileHelper.Current.ReadXmlObjectAsync<List<Duan>>("duan.xml");

            if (duan_list == null) duan_list = new List<Duan>();
            
            // 检查当前段子是否已经收藏
            if (! duan_list.Exists(t => t.DuanID == d.DuanID)) // 未收藏，则加入收藏
            {
                // 增加当前段子到收藏列表
                duan_list.Add(d);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<Duan>>(duan_list, "duan.xml");

                // 收藏成功通知
                PopupMessage(2000, "收藏成功！");
            }
            else // 已收藏，则取消收藏
            {
                duan_list.RemoveAll(f => f.DuanID == d.DuanID);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<Duan>>(duan_list, "duan.xml");

                // 取消收藏成功通知
                PopupMessage(2000, "取消收藏成功！");
            }            
        }
        #endregion

        private async void PopupMessage(int ms, string msg)
        {
            popText.Text = msg;
            popText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double L = popTips.ActualWidth;
            double l = popText.ActualWidth;           
            PopBorder.Margin = new Thickness((L - l) / 2, 0, 0, 0);

            popTips.IsOpen = true;
            await Task.Delay(ms);
            popTips.IsOpen = false;
        }

        private void DuanSplitView_PaneClosed(SplitView sender, object args)
        {
            CommentControl.ClearResponse();            
        }

        private void RelativePanel_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            RefreshPage();
        }

        private void showUnwelcome_Toggled(object sender, RoutedEventArgs e)
        {
            if (showUnwelcome.IsOn)
            {
                PopupMessage(2000, "显示不受欢迎的段子");
                //_popupMsg.ShowAsync("显示不受欢迎的段子", 2000);
            }
            else
            {
                PopupMessage(2000, "隐藏不受欢迎的段子");
                //_popupMsg.ShowAsync("隐藏不受欢迎的段子", 2000);
            }

            RefreshPage();
        }

        private void sliderFontSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            RefreshPage();
        }
    }
}
