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

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (just_returned)
            //{
            //    just_returned = false;
            //    return;
            //}

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
    }
}
