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
            if (parameters != null)
            {
                this.DataContext = _viewModel = new PicDetailViewModel(parameters[0] as BoringPic);
                DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();

                _dViewModel.Update(_viewModel.BoringPicture.PicID);
            }
        }

        private void PageBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PicDownload_Click1(object sender, RoutedEventArgs e)
        {
            var pics = PicListView.Items;
            foreach (ImageUrl url in pics)
            {
                var fileName = Regex.Replace(url.URL, @".+?/", "");

                var path = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await path.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
                var fileStream = await folder.OpenStreamForReadAsync(fileName);

                List<Byte> allBytes = new List<Byte>();
                byte[] buffer = new byte[4000];
                int bytesRead = 0;
                while((bytesRead=await fileStream.ReadAsync(buffer, 0, 4000))>0)
                {
                    allBytes.AddRange(buffer.Take(bytesRead));
                }

                StorageFolder sf = KnownFolders.SavedPictures;
                var saveFile = await sf.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBytesAsync(saveFile, allBytes.ToArray());
                //var file = await folder.CreateFileAsync(imageUrl, CreationCollisionOption.ReplaceExisting);
            }
        }

        private async void PicDownload_Click(object sender, RoutedEventArgs e)
        {
            var pics = _viewModel.BoringPicture;

            foreach (var url in pics.Urls)
            {
                var fileName = Regex.Replace(url.URL, @".+?/", "");

                List<Byte> allBytes = new List<Byte>();
                var path = Windows.Storage.ApplicationData.Current.LocalFolder;
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
        }
    }
}
