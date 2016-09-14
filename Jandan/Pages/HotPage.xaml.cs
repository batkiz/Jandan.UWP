using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using Jandan.UWP.UI;
using System;
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
using System.Threading.Tasks;
using Windows.UI;
using Windows.ApplicationModel.DataTransfer;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HotPage : Page
    {
        HotViewModel _viewModel;

        public HotPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new HotViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.HotPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
        }

        private void BoringGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(ImageViewer), new object[] { e.ClickedItem as BoringPic, PicDetailType.Hot, _viewModel.Pics });
        }

        private void BestCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(FreshDetailPage), new object[] { 1, e.ClickedItem as BestFreshComment });
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
                await PopupMessage("网络不好，请稍后重试", 90, 2000);

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

                    await PopupMessage("感谢您的OO！", 64, 2000);
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (boring.VoteNegative++).ToString();
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

        private async Task PopupMessage(string message, double textWidth, int disTime)
        {
            textBlockPopup.Text = message;
            popTipVote.HorizontalOffset = -textWidth;
            popTipVote.IsOpen = true;   // 提示再按一次
            await Task.Delay(disTime);  // 1000ms后关闭提示
            popTipVote.IsOpen = false;
        }

        private void DuanViewComment_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;

            CommentControl.Update(bp.PicID);
            DuanSplitView.IsPaneOpen = true;
            CommentControl.SetFocus();
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
        private void Duan_Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            CommentControl.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;
            CommentControl.SetFocus();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateHotPics();
            _viewModel.UpdateHotDuan();
            _viewModel.UpdateHotComm();
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void DuanSplitView_PaneClosed(SplitView sender, object args)
        {
            CommentControl.ClearResponse();
        }
    }
}
