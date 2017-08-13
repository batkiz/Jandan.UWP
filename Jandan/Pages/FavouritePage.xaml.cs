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
using Jandan.UWP.Core.ViewModels;
using Jandan.UWP.Core.Models;
using Windows.ApplicationModel.DataTransfer;
using Jandan.UWP.Core.Tools;
using System.Collections.ObjectModel;
using Jandan.UWP.Core.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavouritePage : Page
    {
        FavouriteViewModel ViewModel { get; } = new FavouriteViewModel();

        public FavouritePage()
        {
            this.InitializeComponent();

            this.DataContext = ViewModel = new FavouriteViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.FavouritePage;
            
            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);

            if (DataShareManager.Current.CurrentPageIndex == DataShareManager.Current.PreviousPageIndex)
            {
                RefreshPage();
            }

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void RefreshPage()
        {
            ViewModel.Update();
        }

        private void FreshPullToRefresh_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }

        private void FreshGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(FreshDetailPage), new object[] { 0, e.ClickedItem as Core.Models.Fresh });
        }
        
        private void DuanComment_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            CommentControl.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;
            CommentControl.SetFocus();
        }

        private void BoringComment_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var boring = b.DataContext as BoringPic;

            CommentControl.Update(boring.PicID);
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

        private void MenuFlyoutCopy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            string copied_content = $"转自煎蛋网:\n作者:{d.Author}\nID:{d.DuanID}\n{d.Content}\n(jandan.net | 地球没有新鲜事)";
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(copied_content);
            Clipboard.SetContent(dataPackage);

            PopupMessage(2000, "复制成功");
        }

        private void pullToRefreshBar_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }

        private async void DuanFavButton_Click(object sender, RoutedEventArgs e)
        {
            var m = sender as Button;
            var d = m.DataContext as Duan;

            if (m.Content.ToString() == "取消收藏")
            {
                var duan_list = await FileHelper.Current.ReadXmlObjectAsync<List<Duan>>("duan.xml");

                duan_list.RemoveAll(f => f.DuanID == d.DuanID);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<Duan>>(duan_list, "duan.xml");

                m.Content = "收藏";
                PopupMessage(2000, "取消收藏成功");
            }
            else
            {
                var duan_list = await FileHelper.Current.ReadXmlObjectAsync<List<Duan>>("duan.xml");

                duan_list.Add(d);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<Duan>>(duan_list, "duan.xml");

                m.Content = "取消收藏";
                PopupMessage(2000, "再次收藏成功");
            }
        }
        
        private async void BoringFavButton_Click(object sender, RoutedEventArgs e)
        {
            var m = sender as Button;
            var d = m.DataContext as BoringPic;

            if (m.Content.ToString() == "取消收藏")
            {
                var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("boring.xml");

                boring_list.RemoveAll(f => f.PicID == d.PicID);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<BoringPic>>(boring_list, "boring.xml");

                m.Content = "收藏";
                PopupMessage(2000, "取消收藏成功");
            }
            else
            {
                var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("boring.xml");

                boring_list.Add(d);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<BoringPic>>(boring_list, "boring.xml");

                m.Content = "取消收藏";
                PopupMessage(2000, "再次收藏成功");
            }
        }

        private async void GirlFavButton_Click(object sender, RoutedEventArgs e)
        {
            var m = sender as Button;
            var d = m.DataContext as BoringPic;

            if (m.Content.ToString() == "取消收藏")
            {
                var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("girl.xml");

                boring_list.RemoveAll(f => f.PicID == d.PicID);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<BoringPic>>(boring_list, "girl.xml");

                m.Content = "收藏";
                PopupMessage(2000, "取消收藏成功");
            }
            else
            {
                var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("girl.xml");

                boring_list.Add(d);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<BoringPic>>(boring_list, "girl.xml");

                m.Content = "取消收藏";
                PopupMessage(2000, "再次收藏成功");
            }
        }

        private async void BoringGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("boring.xml");
            ObservableCollection<BoringPic> imageList = new ObservableCollection<BoringPic>(boring_list);
            this.Frame.Navigate(typeof(ImageViewer), new object[] { e.ClickedItem as BoringPic, PicDetailType.Boring, imageList });
        }

        private async void GirlGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("girl.xml");
            ObservableCollection<BoringPic> imageList = new ObservableCollection<BoringPic>(boring_list);
            this.Frame.Navigate(typeof(ImageViewer), new object[] { e.ClickedItem as BoringPic, PicDetailType.Meizi, imageList });
        }

        private async void PopupMessage(int ms, string msg)
        {
            popText.Text = msg;
            popText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double L = popTips.ActualWidth;
            double l = popText.ActualWidth;
            PopBorder.Margin = new Thickness((L - l) / 2, 0, 0, 0);

            popTips.IsOpen = true;
            await System.Threading.Tasks.Task.Delay(ms);
            popTips.IsOpen = false;
        }
    }
}
