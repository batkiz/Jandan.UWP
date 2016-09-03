using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
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
        FreshCommentViewModel _dViewModel;

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
                DuanCommentListView.DataContext = _dViewModel = new FreshCommentViewModel();
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

            _dViewModel.Update(commentId);

            DuanSplitView.IsPaneOpen = true;
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
