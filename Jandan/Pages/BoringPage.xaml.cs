using Jandan.UWP.Control;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class BoringPage : Page
    {
        /// <summary>
        /// 无聊图的View Model
        /// </summary>
        BoringViewModel _viewModel;

        /// <summary>
        /// 用于进入妹子图的计数
        /// </summary>
        private int secret_count;

        public BoringPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new BoringViewModel();
        }
        /// <summary>
        /// 从其他页面导航回到无聊图
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.BoringPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            // 重置妹子图点击计数
            secret_count = 0;

            base.OnNavigatedTo(e);
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
            var boring = b.DataContext as BoringPic;
            var c = b.Parent as RelativePanel;

            var msg = await _viewModel.Vote(boring, isLike);

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
                    t.Text = (boring.VotePositive++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b1 = c.Children[0] as Button;
                    b1.Foreground = new SolidColorBrush(Colors.Red);

                    PopupMessage(2000, "感谢您的OO！");
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (boring.VoteNegative++).ToString();
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

        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;
            
            CommentControl.Update(bp.PicID);

            DuanSplitView.IsPaneOpen = true;

            CommentControl.SetFocus();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void PageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            secret_count++;

            if (secret_count == 5)
            {
                this.Frame.Navigate(typeof(GirlsPage));
                secret_count = 0;
                return;
            }
            
            PopupMessage(2000, $"再点击{5 - secret_count}次进入妹子图");
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void DuanSplitView_PaneClosed(SplitView sender, object args)
        {
            CommentControl.ClearResponse();
        }

        private void BoringGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic, PicDetailType.Boring, _viewModel.Boring });
            this.Frame.Navigate(typeof(ImageViewer), new object[] { e.ClickedItem as BoringPic, PicDetailType.Boring, _viewModel.Boring });
        }

        private void BoringPullToRefresh_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }

        private void showNSFW_Toggled(object sender, RoutedEventArgs e)
        {
            if (showNSFW.IsOn)
            {
                PopupMessage(2000, "显示NSFW图片");
            }
            else
            {
                PopupMessage(2000, "隐藏NSFW图片");
            }

            RefreshPage();
        }

        private void showUnwelcome_Toggled(object sender, RoutedEventArgs e)
        {
            if (showUnwelcome.IsOn)
            {
                PopupMessage(2000, "显示不受欢迎的图片");
            }
            else
            {
                PopupMessage(2000, "隐藏不受欢迎的图片");
            }

            RefreshPage();
        }

        private void RefreshPage()
        {
            _viewModel.UpdateBoringPics();
        }
    }
}
