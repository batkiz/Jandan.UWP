using Jandan.UWP.Control;
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
using Windows.Security.Authentication.Web;
using System.Diagnostics;
using Jandan.UWP.Core.HTTP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        SettingViewModel ViewModel { get; } = new SettingViewModel();

        public SettingPage()
        {
            this.InitializeComponent();

            if (DataShareManager.Current.IsCortanaRegistered)
            {
                btnRegCortana.Content = "已注册";
                btnRegCortana.IsEnabled = false;
            }
            if (DataShareManager.Current.IsLiveTileRegistered)
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
            this.DataContext = ViewModel;
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
            if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.DeniedBySystemPolicy)
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
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
#endif
            }
        }

        private async void btnVote_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH4NN0X"));
        }

        private void tsDarkMode_Toggled(object sender, RoutedEventArgs e)
        {
            ViewModel.ExchangeDarkMode((sender as ToggleSwitch).IsOn);
        }

        private void tsNoImagesMode_Toggled(object sender, RoutedEventArgs e)
        {
            ViewModel.ExchangeNoImageMode((sender as ToggleSwitch).IsOn);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ViewModel.ExchangeAutoDarkMode((sender as ToggleSwitch).IsOn);

            if (tsAutoDarkMode.IsOn)
            {
                StartTimePicker.TimeChanged += StartTimePicker_TimeChanged;
                EndTimePicker.TimeChanged += EndTimePicker_TimeChanged;

                DataShareManager.Current.CheckAppTheme();
            }
            else
            {
                StartTimePicker.TimeChanged -= StartTimePicker_TimeChanged;
                EndTimePicker.TimeChanged -= EndTimePicker_TimeChanged;
            }
        }

        private async void StartTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            var t0 = e.NewTime;
            var t1 = EndTimePicker.Time;

            if (await CheckTime(t0, t1))
            {
                DataShareManager.Current.UpdateStartTime(StartTimePicker.Time);
            }
            else
            {
                var tp = sender as TimePicker;
                tp.Time = e.OldTime;
            }
        }

        private async void EndTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            var t0 = StartTimePicker.Time;
            var t1 = e.NewTime;

            if (await CheckTime(t0, t1))
            {
                DataShareManager.Current.UpdateEndTime(EndTimePicker.Time);
            }
            else
            {
                var tp = sender as TimePicker;
                tp.Time = e.OldTime;
            }
        }

        private async Task<bool> CheckTime(TimeSpan t0, TimeSpan t1)
        {
            if (TimeSpan.Compare(t0, t1) == 0)
            {
                var dialog = new ContentDialog()
                {
                    Title = "提示",
                    Content = "结束时间不能与开始时间相同，请重新设置",
                    PrimaryButtonText = "确定",
                    FullSizeDesired = false,
                    RequestedTheme = DataShareManager.Current.AppTheme
                };

                dialog.PrimaryButtonClick += (_s, _e) => { };
                await dialog.ShowAsync();

                return false;
            }
            else
            {
                return true;
            }
        }

        private void Printlog(string info)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + info);
#endif
        }
        private async void btnIDandEmail_Click(object sender, RoutedEventArgs e)
        {
            var dia = new ContentDialog()
            {
                Title = "提示",
                Content = new CommentSubmitDialogue(DataShareManager.Current.UserName, DataShareManager.Current.EmailAdd),
                PrimaryButtonText = "确认",
                SecondaryButtonText = "取消",
                FullSizeDesired = false,
                RequestedTheme = DataShareManager.Current.AppTheme
            };
            dia.PrimaryButtonClick += Dia_PrimaryButtonClick;

            await dia.ShowAsync();            
        }

        private void Dia_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var csd = sender.Content as CommentSubmitDialogue;

            DataShareManager.Current.UpdateUserName(csd.UserName);
            DataShareManager.Current.UpdateEmailAdd(csd.Email);
        }

        private static void ShowFlyout(object sender)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }
                
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            //var s = sender as MenuFlyoutItem;

            //await _viewModel.GetAuthAsync(s.Text);
            //_viewModel.Update();
        }

       
        private void comboxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var cbi = cb.SelectedItem as ComboBoxItem;
            var s = cbi.Content.ToString();

            switch (s.ToLower())
            {
                case "small":
                    ViewModel.SwitchFontSize(PageFontSize.Small);
                    break;
                default:
                case "normal":
                    ViewModel.SwitchFontSize(PageFontSize.Normal);
                    break;
                case "large":
                    ViewModel.SwitchFontSize(PageFontSize.Large);
                    break;
            }
        }
    }
}
