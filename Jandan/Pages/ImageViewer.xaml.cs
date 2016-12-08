﻿using Jandan.UWP.Control;
using Jandan.UWP.Core.Data;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageViewer : Page
    {
        PicDetailViewModel _viewModel;

        object ItemList;
        BoringPic CurrentItem;
        PicDetailType DetailType;

        DataTransferManager dataTransferManager;

        public ImageViewer()
        {
            this.InitializeComponent();

            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;            
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "煎蛋网 - 无聊图";
            request.Data.Properties.Description = $"来自煎蛋网分享的无聊图，ID{CurrentItem.PicID}";
            request.Data.SetText($"来自煎蛋网的分享：{CurrentItem.Content}");

            var pics = _viewModel.BoringPicture;
            var url = pics.Urls[0];
            
            //To ensure image is cached
            var bitmapimage = await ImageCache.Instance.GetFromCacheAsync(new Uri(url.URL));
            //Get the name of the file
            var name = CreateHash64(url.URL).ToString();
            //Get the ImageCache Folder
            var folder = await ApplicationData.Current.TemporaryFolder.GetFolderAsync("ImageCache");
            //Get the StorageFile
            var file = await folder.GetFileAsync(name);

            var fileStream = await file.OpenStreamForReadAsync();

            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(fileStream.AsRandomAccessStream()));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.PicDetailPage;

            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters[0] != null)
            {
                var p = parameters[0] as BoringPic;
                this.DataContext = _viewModel = new PicDetailViewModel(p);

                CurrentItem = p;
                DetailType = (PicDetailType)parameters[1];
                ItemList = parameters[2];
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += PicDetailPage_BackRequested;            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager.GetForCurrentView().BackRequested -= PicDetailPage_BackRequested;
        }

        private void PicDetailPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            OnBackRequested();
        }

        private void OnBackRequested()
        {
            switch (DetailType)
            {
                case PicDetailType.Boring:
                    this.Frame.Navigate(typeof(BoringPage));
                    break;
                case PicDetailType.Hot:
                    this.Frame.Navigate(typeof(HotPage));
                    break;
                case PicDetailType.Meizi:
                    this.Frame.Navigate(typeof(GirlsPage));
                    break;
                default:
                    break;
            }
        }

        private void PageBackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackRequested();
        }

        private async void PicDownload_Click(object sender, RoutedEventArgs e)
        {
            var pics = _viewModel.BoringPicture;

            foreach (var url in pics.Urls)
            {
                var fileName = Regex.Replace(url.URL, @".+?/", "");

                List<Byte> allBytes = new List<Byte>();
                try
                {
                    //To ensure image is cached
                    var bitmapimage = await ImageCache.Instance.GetFromCacheAsync(new Uri(url.URL));
                    //Get the name of the file
                    var name = CreateHash64(url.URL).ToString();
                    //Get the ImageCache Folder
                    var folder = await ApplicationData.Current.TemporaryFolder.GetFolderAsync("ImageCache");
                    //Get the StorageFile
                    var file = await folder.GetFileAsync(name);

                    StorageFolder sf = KnownFolders.SavedPictures;
                    string newName = pics.PicID + $"-[{pics.Urls.IndexOf(url)}]" + Path.GetExtension(fileName);
                    var saveFile = await sf.CreateFileAsync(newName, CreationCollisionOption.ReplaceExisting);

                    await file.CopyAndReplaceAsync(saveFile);
                }
                catch (Exception)
                {
                    return;
                }                
            }

            await PopupMessage("已经保存到图片文件夹", 80, 2000);
        }

        private static ulong CreateHash64(string str)
        {
            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(str);
            ulong value = (ulong)utf8.Length;
            for (int n = 0; n < utf8.Length; n++)
            {
                value += (ulong)utf8[n] << ((n * 5) % 56);
            }
            return value;
        }

        private void DuanCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var d = e.ClickedItem as DuanComment;
            var user_name = d.AuthorName;
            var vm = _viewModel._dViewModel;

            vm.TextBoxComment = $"@{user_name}: ";
            vm.ParentId = d.PostID;
        }

        private async void CommentSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = _viewModel._dViewModel;
            var response = vm.TextBoxComment;

            if (!response.StartsWith("@") && !string.IsNullOrEmpty(vm.ParentId))
            {
                vm.ParentId = "";
            }

            var dia = new ContentDialog()
            {
                Title = "提示",
                Content = new CommentSubmitDialogue(DataShareManager.Current.UserName, DataShareManager.Current.EmailAdd),
                PrimaryButtonText = "发送",
                SecondaryButtonText = "取消",
                FullSizeDesired = false,
                RequestedTheme = DataShareManager.Current.AppTheme
            };
            dia.PrimaryButtonClick += Dia_PrimaryButtonClick;

            var result = await dia.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var message = $"message={response}&thread_id={vm.ThreadId}&parent_id={vm.ParentId}&author_name={DataShareManager.Current.UserName}&author_email={DataShareManager.Current.EmailAdd}";

                var r = await vm.PostComment(message);

                vm.TextBoxComment = "";

                JsonObject j = new JsonObject();
                if (JsonObject.TryParse(r, out j))
                {
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "评论成功！");
                }
            }
        }

        private void Dia_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var csd = sender.Content as CommentSubmitDialogue;

            DataShareManager.Current.UserName = csd.UserName;
            DataShareManager.Current.EmailAdd = csd.Email;
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
                this.Frame.Navigate(typeof(ImageViewer), new object[] { list.ElementAt(idx - 1), DetailType, ItemList });
            }
            else
            {
                await PopupMessage("已经是第一张了哦", 65, 2000);
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
                this.Frame.Navigate(typeof(ImageViewer), new object[] { list.ElementAt(idx + 1), DetailType, ItemList });
            }
            else if (DetailType == PicDetailType.Boring)
            {
                var b = ItemList as BoringIncrementalLoadingCollection;
                await b.LoadMoreItemsAsync(0);
                this.Frame.Navigate(typeof(ImageViewer), new object[] { b.ElementAt(idx + 1), DetailType, b });
            }
            else if (DetailType == PicDetailType.Meizi)
            {
                var b = ItemList as MeiziIncrementalLoadingCollection;
                await b.LoadMoreItemsAsync(0);
                this.Frame.Navigate(typeof(ImageViewer), new object[] { b.ElementAt(idx + 1), DetailType, b });
            }
            else
            {
                await PopupMessage("已经是最后一张了哦", 68, 2000);
            }
        }

        private async Task PopupMessage(string message, double textWidth, int disTime)
        {
            popupMsg.Text = message;
            popTips.HorizontalOffset = -textWidth;
            popTips.IsOpen = true;
            await Task.Delay(disTime);
            popTips.IsOpen = false;
        }
    }
    
}
