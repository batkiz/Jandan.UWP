using Jandan.UWP.Core.ViewModels;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        SettingViewModel _viewModel;

        public SettingPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 从其他页面导航到“关于”页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.SettingPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
            this.DataContext = _viewModel = new SettingViewModel();

        }

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

        private void btnRegLiveTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRegCortana_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnVote_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tsDarkMode_Toggled(object sender, RoutedEventArgs e)
        {
            _viewModel.ExchangeDarkMode((sender as ToggleSwitch).IsOn);
        }

        private void tsNoImagesMode_Toggled(object sender, RoutedEventArgs e)
        {
            _viewModel.ExchangeNoImageMode((sender as ToggleSwitch).IsOn);
        }
    }
}
