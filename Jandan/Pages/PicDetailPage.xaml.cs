using Jandan.UWP.Control;
using Jandan.UWP.Core.Data;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan.UWP.UI
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

        DataTransferManager dataTransferManager;

        public PicDetailPage()
        {
            this.InitializeComponent();

            // 分享
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

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

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "煎蛋网 - 无聊图";
            request.Data.Properties.Description = $"来自煎蛋网分享的无聊图，ID{CurrentItem.PicID}";
            request.Data.SetText($"来自煎蛋网的分享：{CurrentItem.Content}");

            var pics = _viewModel.BoringPicture;
            var url = pics.Urls[0];
            var fileName = Regex.Replace(url.URL, @".+?/", "");
            var path = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
            var folder = await path.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
            var fileStream = await folder.OpenStreamForReadAsync(fileName);

            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(fileStream.AsRandomAccessStream())); 
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
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

            SystemNavigationManager.GetForCurrentView().BackRequested += PicDetailPage_BackRequested;

            //////////////////////////////////////////////
            CommentSubmitButton.Focus(FocusState.Pointer);
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
                        
            await PopupMessage("已经保存到图片文件夹", 80, 2000);
        }

        private async Task PopupMessage(string message, double textWidth, int disTime)
        {
            popupMsg.Text = message;
            popTips.HorizontalOffset = -textWidth;
            popTips.IsOpen = true;   // 提示再按一次
            await Task.Delay(disTime);  // 1000ms后关闭提示
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

        private async void CommentSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var response = CommentInputTextBox.Text;

            if (!response.StartsWith("@") && !string.IsNullOrEmpty(_dViewModel.ParentId))
            {
                _dViewModel.ParentId = "";
            }

            var dia = new ContentDialog()
            {
                Title = "提示",
                Content = new CommentSubmitDialogue(DataShareManager.Current.UserName, DataShareManager.Current.EmailAdd),
                PrimaryButtonText = "发送",
                SecondaryButtonText = "取消",
                FullSizeDesired = false
            };
            dia.PrimaryButtonClick += Dia_PrimaryButtonClick;

            var result = await dia.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var message = $"message={response}&thread_id={_dViewModel.ThreadId}&parent_id={_dViewModel.ParentId}&author_name={DataShareManager.Current.UserName}&author_email={DataShareManager.Current.EmailAdd}";

                var r = await _dViewModel.PostComment(message);

                JsonObject j = new JsonObject();
                if (JsonObject.TryParse(r, out j))
                {
                    await PopupMessage("评论成功！", 40, 2000);
                }

                CommentInputTextBox.Text = "";
            }
        }

        private void Dia_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var csd = sender.Content as CommentSubmitDialogue;

            DataShareManager.Current.UserName = csd.UserName;
            DataShareManager.Current.EmailAdd = csd.Email;
        }

        private void DuanCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var d = e.ClickedItem as DuanComment;
            var user_name = d.AuthorName;

            CommentInputTextBox.Text = $"@{user_name}: ";

            _dViewModel.ParentId = d.PostID;
        }
    }
}
