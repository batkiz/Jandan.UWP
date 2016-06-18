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
using Jandan.UWP.ViewModels;
using Jandan.UWP.Models;
using ImageLib.Controls;
using Windows.Storage;
using Windows.ApplicationModel;
using System.Text.RegularExpressions;
using Jandan.UWP.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PicDetailPage : Page
    {
        PicDetailViewModel _viewModel;
        DuanCommentViewModel _dViewModel;

        object ItemList;
        BoringPic CurrentItem;
        PicDetailType DetailType;

        public PicDetailPage()
        {
            this.InitializeComponent();
            
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (string.Equals(platformFamily, "Windows.Mobile"))
            {
                PicListView.MinWidth = PicDetailPanel.ActualWidth;
            }
            else
            {
                PicListView.MinWidth = 500;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.PicDetailPage;

            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters[0] != null)
            {
                var p = parameters[0] as BoringPic;
                DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
                this.DataContext = _viewModel = new PicDetailViewModel(p);
                
                _dViewModel.Update(p.PicID);

                CurrentItem = p;
                DetailType = (PicDetailType)parameters[1];
                ItemList = parameters[2];
            }
        }

        private void PageBackButton_Click(object sender, RoutedEventArgs e)
        {
            switch (DetailType)
            {
                case PicDetailType.Boring:
                    this.Frame.Navigate(typeof(BoringPicsPage), new object[] { 0 });
                    break;
                case PicDetailType.Hot:
                    this.Frame.Navigate(typeof(HotPage), new object[] { 1 });
                    break;
                case PicDetailType.Meizi:
                    this.Frame.Navigate(typeof(MeiziPicsPage));
                    break;
                default:
                    break;
            }
            //if (this.Frame.CanGoBack)
            //{
            //    Frame.GoBack();                
            //}
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PicDownload_Click(object sender, RoutedEventArgs e)
        {
            var pics = _viewModel.BoringPicture;

            foreach (var url in pics.Urls)
            {
                var fileName = Regex.Replace(url.URL, @".+?/", "");

                List<Byte> allBytes = new List<Byte>();
                var path = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                var folder = await path.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
                try
                {
                    var fileStream = await folder.OpenStreamForReadAsync(fileName);
                    
                    byte[] buffer = new byte[4000];
                    int bytesRead = 0;
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, 4000)) > 0)
                    {
                        allBytes.AddRange(buffer.Take(bytesRead));
                    }
                }
                catch (Exception)
                {
                    return;
                }                
                
                StorageFolder sf = KnownFolders.SavedPictures;
                string newName = pics.PicID + $"-[{pics.Urls.IndexOf(url)}]" + Path.GetExtension(fileName);
                var saveFile = await sf.CreateFileAsync(newName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBytesAsync(saveFile, allBytes.ToArray());
            }

            popupMsg.Text = "已经保存到图片文件夹";
            popTips.IsOpen = true;   // 提示再按一次
            await Task.Delay(1000);  // 1000ms后关闭提示
            popTips.IsOpen = false;
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            PreviousPic();
        }

        private async void PreviousPic()
        {
            var list = ItemList as ObservableCollection<BoringPic>;
            var idx = list.IndexOf(CurrentItem);
            if (idx != 0)
            {
                this.Frame.Navigate(typeof(PicDetailPage), new object[] { list.ElementAt(idx - 1), DetailType, ItemList });
            }
            else
            {
                popupMsg.Text = "已经是第一张了哦";
                popTips.IsOpen = true;   // 提示再按一次
                await Task.Delay(1000);  // 1000ms后关闭提示
                popTips.IsOpen = false;
            }
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            NextPic();
        }

        private async void NextPic()
        {
            var list = ItemList as ObservableCollection<BoringPic>;
            var idx = list.IndexOf(CurrentItem);
            if (idx != list.Count - 1)
            {
                this.Frame.Navigate(typeof(PicDetailPage), new object[] { list.ElementAt(idx + 1), DetailType, ItemList });
            }
            else if (DetailType == PicDetailType.Boring)
            {
                var b = ItemList as BoringIncrementalLoadingCollection;
                await b.LoadMoreItemsAsync(0);
                this.Frame.Navigate(typeof(PicDetailPage), new object[] { b.ElementAt(idx + 1), DetailType, b });
            }
            else if (DetailType == PicDetailType.Meizi)
            {
                var b = ItemList as MeiziIncrementalLoadingCollection;
                await b.LoadMoreItemsAsync(0);
                this.Frame.Navigate(typeof(PicDetailPage), new object[] { b.ElementAt(idx + 1), DetailType, b });
            }
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            double delta = e.Cumulative.Translation.X;

            if (delta > 100)
            {
                PreviousPic();
                return;
            }
            else if (delta < -100)
            {
                NextPic();
                return;
            }
        }
    }
}
