using Jandan.UWP.Core.Tools;
using Jandan.UWP.Core.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Jandan.UWP.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MainViewModel ViewModel { get; } = new MainViewModel();        

        public MainPage()
        {
            this.InitializeComponent();
            
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            DispatcherManager.Current.Dispatcher = Dispatcher;
        }       

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (DataShareManager.Current.CurrentPageIndex == PageIndex.FreshPage)
            {
                if (popTips.IsOpen)  //第二次按back键
                {
                    Application.Current.Exit();
                }
                else
                {
                    PopupMessage(1000, "再按一次退出");
                    e.Handled = true;
                    //popTips.IsOpen = true;   // 提示再按一次
                    //await Task.Delay(1000);  // 1000ms后关闭提示
                    //popTips.IsOpen = false;
                }
            }            
            else if (DataShareManager.Current.PreviousPageIndex == PageIndex.FreshDetailPage
                || (DataShareManager.Current.CurrentPageIndex != PageIndex.MeiziPage && DataShareManager.Current.PreviousPageIndex == PageIndex.PicDetailPage)
                || (DataShareManager.Current.CurrentPageIndex != PageIndex.BoringPage && DataShareManager.Current.PreviousPageIndex == PageIndex.PicDetailPage)
                || (DataShareManager.Current.PreviousPageIndex == PageIndex.MeiziPage && DataShareManager.Current.CurrentPageIndex != PageIndex.PicDetailPage))
            {
                mainFrame.Navigate(typeof(FreshPage));
            }
            else
            {
                mainFrame.GoBack();
            }

            e.Handled = true;
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataShareManager.Current.CurrentPageIndex = PageIndex.MainPage;

            this.DataContext = ViewModel;

            mainFrame.Navigate(typeof(FreshPage));
        }

        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_clicked(object sender, RoutedEventArgs e)
        {
            var itemName = sender as AppBarButton;

            JumpToPage(itemName.Name);

            MainCommandBar.IsOpen = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ////需要添加windows mobile extensions for uwp引用
            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            //{
            //    StatusBar statusBar = StatusBar.GetForCurrentView();
            //    statusBar.BackgroundColor = Color.FromArgb(100, 255, 255, 255);
            //    statusBar.ForegroundColor = Colors.White;
            //    statusBar.BackgroundOpacity = 1;
            //}

            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (string.Equals(platformFamily, "Windows.Mobile"))
            {
                appView.FullScreenSystemOverlayMode = FullScreenSystemOverlayMode.Standard;
                appView.TryEnterFullScreenMode();
            }
        }

        private void SecBtnDarkMode_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ToggleAPPTheme();
        }

        private void SecBtnAbout_Click(object sender, RoutedEventArgs e)
        {
            this.mainFrame.Navigate(typeof(AboutPage));
        }

        private void SecBtnSetting_Click(object sender, RoutedEventArgs e)
        {
            this.mainFrame.Navigate(typeof(SettingPage));
        }

        private async void SecBtnClearCache_Click(object sender, RoutedEventArgs e)
        {            
            await FileHelper.Current.DeleteCacheFile();
        }

        private async void MainCommandBar_Opening(object sender, object e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var dmon = loader.GetString("DarkModeOn");
            var dmof = loader.GetString("DarkModeOff");
            var nws = loader.GetString("NetworkConnection");
            var cuc = loader.GetString("CleanupCache");

            SecBtnDarkMode.Label = ViewModel.AppTheme == ElementTheme.Dark ? dmof : dmon;

            // 检查当前缓存使用
            var size = await FileHelper.Current.GetCacheSize();
            SecBtnSetting.Label = $"{cuc} ({FileHelper.Current.GetFormatSize(size)})";

            // 显示当前网络状态
            NetStatus.Label = $"{nws} ({ConnectivityHelper.NetworkStatus()})";


        }

        private void HamburgerMainMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedButton = e.ClickedItem as HamburgerMenuGlyphItem;

            JumpToPage(clickedButton.Label);

            if (HamburgerMainMenu.HamburgerVisibility == Visibility.Visible)
            {
                HamburgerMainMenu.IsPaneOpen = false;
            }            
        }

        private void JumpToPage(string label)
        {
            switch (label.ToLower())
            {
                case "新鲜事": case "freshnews": case "fresh":
                    mainFrame.Navigate(typeof(FreshPage), null, new ContinuumNavigationTransitionInfo());
                    break;
                case "无聊图": case "boringpics": case "pics":
                    mainFrame.Navigate(typeof(BoringPage), null, new ContinuumNavigationTransitionInfo());
                    break;
                case "段  子": case "duanzi": case "joke":
                    mainFrame.Navigate(typeof(DuanPage), null, new ContinuumNavigationTransitionInfo());
                    break;
                case "热  榜": case "hot":
                    mainFrame.Navigate(typeof(HotPage), null, new ContinuumNavigationTransitionInfo());
                    break;
                case "收  藏": case "favourite": case "favorite":
                    mainFrame.Navigate(typeof(FavouritePage), null, new ContinuumNavigationTransitionInfo());
                    break;
                case "设  置":case "settings":
                    mainFrame.Navigate(typeof(SettingPage), null, new ContinuumNavigationTransitionInfo());
                    break;
                case "关  于":case "about":
                    mainFrame.Navigate(typeof(AboutPage), null, new ContinuumNavigationTransitionInfo());
                    break;
                default:
                    break;
            }
        }

        private async void PopupMessage(int ms, string msg)
        {
            popText.Text = msg;

            double L = popTips.ActualWidth;
            double l = popText.ActualWidth;
            PopBorder.Margin = new Thickness((L - l) / 2, 0, 0, 0);

            popTips.IsOpen = true;   // 提示再按一次
            await Task.Delay(ms);  // 1000ms后关闭提示
            popTips.IsOpen = false;
        }
    }
}
