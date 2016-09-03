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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan.UWP.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        AboutViewModel _viewModel;

        public AboutPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 从其他页面导航到“关于”页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.AboutPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
            this.DataContext = _viewModel = new AboutViewModel();

        }
        /// <summary>
        /// 点击页面左上角的返回箭头按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageBackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Frame.GoBack();
            }
            catch (Exception)
            {

            }
        }
    }

}
