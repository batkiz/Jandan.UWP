using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class FreshDetailPage : Page
    {
        private FreshDetailViewModel _viewModel;
        DataTransferManager dataTransferManager;

        public FreshDetailPage()
        {
            this.InitializeComponent();

            // 分享
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;           
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            var FreshNews = _viewModel.FreshDetails;

            DataRequest request = e.Request;
            request.Data.Properties.Title = "煎蛋网 - 新鲜事";
            request.Data.Properties.Description = $"来自煎蛋网分享的新鲜事，ID{FreshNews.FreshInfo.ID}";
            request.Data.SetText($"来自煎蛋网的分享：\n标题：{FreshNews.FreshInfo.Title}\n日期：{FreshNews.FreshInfo.Date}");
            request.Data.SetHtmlFormat(FreshNews.FreshContentSlim);
            request.Data.SetWebLink(new Uri(FreshNews.FreshInfo.Url));
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.FreshDetailPage;

            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null)
            {
                switch ((int)parameters[0])
                {
                    case 0:
                        this.DataContext = _viewModel = new FreshDetailViewModel(parameters[1] as Fresh);
                        break;
                    case 1:
                        this.DataContext = _viewModel = new FreshDetailViewModel(parameters[1] as BestFreshComment);
                        break;
                    default:
                        break;
                }                
            }
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

        private void FreshCommentButton_Click(object sender, RoutedEventArgs e)
        {
            string commentId = _viewModel.FreshDetails.FreshInfo.ID;

            CommentControl.Update(commentId);
            DuanSplitView.IsPaneOpen = true;

            CommentControl.SetFocus();
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            double delta = e.Cumulative.Translation.X;

            if (delta > 10 && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void FreshFavButton_Click(object sender, RoutedEventArgs e)
        {
            // 读取当前收藏列表
            var fresh_list = await FileHelper.Current.ReadXmlObjectAsync<List<Fresh>>("fresh.xml");

            if (fresh_list == null) fresh_list = new List<Fresh>();

            // 检查当前新鲜事是否已经收藏
            if (!_viewModel.IsFavourite) // 未收藏，则加入收藏
            {
                // 增加当前新鲜事到收藏列表
                fresh_list.Add(_viewModel.FreshDetails.FreshInfo);
                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<Fresh>>(fresh_list, "fresh.xml");

                _viewModel.IsFavourite = true;
                // 收藏成功通知
            }
            else // 已收藏，则取消收藏
            {
                //fresh_list.Remove(_viewModel.FreshDetails.FreshInfo);
                fresh_list.RemoveAll(f => f.ID == _viewModel.FreshDetails.FreshInfo.ID);

                // 写入收藏列表
                await FileHelper.Current.WriteXmlObjectAsync<List<Fresh>>(fresh_list, "fresh.xml");

                _viewModel.IsFavourite = false;
                // 取消收藏成功通知
            }
        }
    }

    class ExtensionHTMLStringProperties
    {
        // "HtmlString" attached property for a WebView
        public static readonly DependencyProperty HtmlStringProperty =
            DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(ExtensionHTMLStringProperties), new PropertyMetadata("", OnHtmlStringChanged));

        // Getter and Setter
        public static string GetHtmlString(DependencyObject obj) { return (string)obj.GetValue(HtmlStringProperty); }
        public static void SetHtmlString(DependencyObject obj, string value) { obj.SetValue(HtmlStringProperty, value); }

        // Handler for property changes in the DataContext : set the WebView
        private static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;
            if (wv != null)
            {
                wv.NavigateToString((string)e.NewValue);
            }
        }
    }
}
