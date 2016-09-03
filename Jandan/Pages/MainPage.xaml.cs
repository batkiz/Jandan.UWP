using Jandan.UWP.Core.Tools;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Threading.Tasks;
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
        MainViewModel _viewModel;        

        public MainPage()
        {
            this.InitializeComponent();
            
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            DispatcherManager.Current.Dispatcher = Dispatcher;
        }       

        private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (DataShareManager.Current.CurrentPageIndex == PageIndex.FreshPage)
            {
                if (popTips.IsOpen)  //第二次按back键
                {
                    Application.Current.Exit();
                }
                else
                {
                    popTips.IsOpen = true;   // 提示再按一次
                    e.Handled = true;
                    await Task.Delay(1000);  // 1000ms后关闭提示
                    popTips.IsOpen = false;
                }
            }            
            else if (DataShareManager.Current.PreviousPageIndex == PageIndex.FreshDetailPage
                || (DataShareManager.Current.CurrentPageIndex != PageIndex.MeiziPage && DataShareManager.Current.PreviousPageIndex == PageIndex.PicDetailPage)
                || (DataShareManager.Current.CurrentPageIndex != PageIndex.BoringPage && DataShareManager.Current.PreviousPageIndex == PageIndex.PicDetailPage)
                || (DataShareManager.Current.PreviousPageIndex == PageIndex.MeiziPage && DataShareManager.Current.CurrentPageIndex != PageIndex.PicDetailPage))
            {
                mainFrame.Navigate(typeof(NewFreshPage));
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

            this.DataContext = _viewModel = new MainViewModel();
            //SetRequestedTheme();

            mainFrame.Navigate(typeof(NewFreshPage));
        }

        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_clicked(object sender, RoutedEventArgs e)
        {
            var itemName = sender as AppBarButton;

            if (string.Equals(itemName.Name, "FreshNews"))
            {
                //mainFrame.Navigate(typeof(FreshPage), null, new ContinuumNavigationTransitionInfo());
                mainFrame.Navigate(typeof(NewFreshPage), null, new ContinuumNavigationTransitionInfo());

            }
            else if (string.Equals(itemName.Name, "Duanzi"))
            {
                mainFrame.Navigate(typeof(DuanPage), null, new ContinuumNavigationTransitionInfo());
            }
            else if (string.Equals(itemName.Name, "BoringPics"))
            {
                mainFrame.Navigate(typeof(NewBoringPage), null, new ContinuumNavigationTransitionInfo());
                //mainFrame.Navigate(typeof(ImageViewer), null, new ContinuumNavigationTransitionInfo());
            }
            else if (string.Equals(itemName.Name, "Hot"))
            {
                mainFrame.Navigate(typeof(NewHotPage), null, new ContinuumNavigationTransitionInfo());
            }

            MainCommandBar.IsOpen = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (string.Equals(platformFamily, "Windows.Mobile"))
            {
                appView.TryEnterFullScreenMode();
            }
            else
            {
                //Hot.Visibility = Visibility.Collapsed;
            }
        }

        private void SecBtnDarkMode_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ToggleAPPTheme();
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
            SecBtnDarkMode.Label = _viewModel.AppTheme == ElementTheme.Dark ? "日间模式" : "夜间模式";

            // 检查当前缓存使用
            var size = await FileHelper.Current.GetCacheSize();
            SecBtnSetting.Label = $"清理缓存（{FileHelper.Current.GetFormatSize(size)}）";

            // 显示当前网络状态
            NetStatus.Label = $"网络状态({NetworkManager.Current.NetworkTitle})";
        }
    }
}
