using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Jandan.UWP.ViewModels;
using Jandan.UWP.Models;
using Windows.UI.Xaml.Documents;
using Windows.UI;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HotPage : Page
    {
        HotViewModel _viewModel;
        DuanCommentViewModel _dViewModel;

        public HotPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.HotPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
            
            this.DataContext = _viewModel = new HotViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
            LoadingCommentProgressBar.DataContext = _dViewModel;
        }

        private void BoringListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic, PicDetailType.Hot, _viewModel.Pics });
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ShowFlyout(sender);
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

        private void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            string copied_content = $"转自煎蛋网:\n作者:{d.Author}\nID:{d.DuanID}\n{d.Content}\n(jandan.net | 地球没有新鲜事)";
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(copied_content);
            Clipboard.SetContent(dataPackage);
        }

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
                textBlockPopup.Text = "网络不好，请稍后重试";
                popTipVote.HorizontalOffset = -90;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;

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

                    textBlockPopup.Text = "感谢您的OO！";
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (duan.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    textBlockPopup.Text = "感谢您的XX！";
                }

                popTipVote.HorizontalOffset = -64;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                textBlockPopup.Text = "您已投过票了";
                popTipVote.HorizontalOffset = -60;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;
            }
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BoringGridView.Visibility == Visibility.Collapsed)
            {
                return;
            }

            double margin = BoringGridView.Padding.Left + BoringGridView.Padding.Right;

            double currentWidth = this.ActualWidth - margin;
            int columnCount = (int)Math.Floor(currentWidth / 230);
            double setWidth = (currentWidth - (columnCount - 1) * 10) / columnCount;

            var s = BoringGridView.ItemContainerStyle;
            Setter s4 = new Setter(GridViewItem.WidthProperty, setWidth);

            Style s_new = new Style(typeof(GridViewItem));
            s_new.Setters.Add(s4);

            BoringGridView.ItemContainerStyle = s_new;
        }

        private void DuanViewComment_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;

            _dViewModel.Update(bp.PicID);
            DuanSplitView.IsPaneOpen = true;
        }

        private void Duan_Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            _dViewModel.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;
        }

        private void BestCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(FreshDetailPage), new object[] { 1, e.ClickedItem as BestFreshComment });
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateHotPics();
            _viewModel.UpdateHotDuan();
            _viewModel.UpdateHotComm();
        }

        private void HotImagePositiveIcon_Click(object sender, RoutedEventArgs e)
        {
            HotImageVote(sender, true);
        }

        private void HotImageNegativeIcon_Click(object sender, RoutedEventArgs e)
        {
            HotImageVote(sender, false);
        }

        private async void HotImageVote(object sender, bool isLike)
        {
            var b = sender as Button;
            var boring = b.DataContext as BoringPic;
            var c = b.Parent as RelativePanel;

            var msg = await _viewModel.Vote(boring, true);

            if (msg == null)
            {
                textBlockPopup.Text = "网络不好，请稍后重试";
                popTipVote.HorizontalOffset = -90;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;

                return;
            }

            if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    var t = c.Children[1] as TextBlock;
                    t.Text = (boring.VotePositive++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b1 = c.Children[0] as Button;
                    b1.Foreground = new SolidColorBrush(Colors.Red);

                    textBlockPopup.Text = "感谢您的OO！";
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (boring.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    textBlockPopup.Text = "感谢您的XX！";
                }

                popTipVote.HorizontalOffset = -64;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                textBlockPopup.Text = "您已投过票了";
                popTipVote.HorizontalOffset = -60;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;
            }
        }
    }
}
