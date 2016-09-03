﻿using Jandan.UWP.Control;
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
    public sealed partial class NewBoringPage : Page
    {
        /// <summary>
        /// 无聊图的View Model
        /// </summary>
        BoringViewModel _viewModel;

        /// <summary>
        /// 用于进入妹子图的计数
        /// </summary>
        private int secret_count;

        public NewBoringPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new BoringViewModel();
            //this.CommentControl.
            //DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
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
            textBlockMeiziCount.Text = message;
            popTipsMeizi.HorizontalOffset = -textWidth;
            popTipsMeizi.IsOpen = true;   // 提示再按一次
            await Task.Delay(disTime);  // 1000ms后关闭提示
            popTipsMeizi.IsOpen = false;
        }

        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;
            
            CommentControl.Update(bp.PicID);

            DuanSplitView.IsPaneOpen = true;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateBoringPics();
        }

        private async void PageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            secret_count++;

            if (secret_count == 5)
            {
                this.Frame.Navigate(typeof(NewGirlsPage));
                secret_count = 0;
                return;
            }

            string tips = $"再点击{5 - secret_count}次进入妹子图";

            await PopupMessage(tips, 75, 2000);
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
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic, PicDetailType.Boring, _viewModel.Boring });
        }
    }
}
