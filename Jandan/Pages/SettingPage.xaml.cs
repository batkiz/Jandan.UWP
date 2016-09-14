using Jandan.UWP.Core.ViewModels;
using Jandan.UWP.LiveTileTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

            if (DataShareManager.Current.isCortanaRegistered)
            {
                btnRegCortana.Content = "已注册";
                btnRegCortana.IsEnabled = false;
            }
            if (DataShareManager.Current.isLiveTileRegistered)
            {
                btnRegLiveTile.Content = "已注册";
                btnRegLiveTile.IsEnabled = false;
            }
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
            RegisterLiveTileTask();

            var b = sender as Button;
            b.Content = "已注册";
            b.IsEnabled = false;
        }

        #region 动态磁贴
        private const string LIVETILETASK = "JandanLiveTileTask";
        private async void RegisterLiveTileTask()
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.Denied)
            {
                return;
            }
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == LIVETILETASK)
                {
                    task.Value.Unregister(true);
                }
            }

            var taskBuilder = new BackgroundTaskBuilder
            {
                Name = LIVETILETASK,
                TaskEntryPoint = typeof(JandanLiveTileTask).FullName
            };
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
            taskBuilder.SetTrigger(new TimeTrigger(60, false));
            taskBuilder.Register();

            DataShareManager.Current.EnableLiveTile();
        }
        #endregion

        private void btnRegCortana_Click(object sender, RoutedEventArgs e)
        {
            InstallCortanaCommand();

            var b = sender as Button;
            b.Content = "已注册";
            b.IsEnabled = false;
        }

        private async void InstallCortanaCommand()
        {
            try
            {
                ////user can stop VCD in settings
                //if (AppSettings.GetInstance().CortanaVCDEnableStatus == false)
                //    return;

                // Install the main VCD. Since there's no simple way to test that the VCD has been imported, or that it's your most recent
                // version, it's not unreasonable to do this upon app load.
                StorageFile vcdStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Cortana/VoiceCommandsFile.xml"));
                await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);

                DataShareManager.Current.EnableCortana();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }
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
