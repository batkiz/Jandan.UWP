using Jandan.UWP.Control;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan.UWP.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HotPage : Page
    {
        HotViewModel _viewModel;
        DuanCommentViewModel _dViewModel;

        public HotPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new HotViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
            LoadingCommentProgressBar.DataContext = _dViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.HotPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);            
        }

        private void BoringListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic, PicDetailType.Hot, _viewModel.Pics });
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ShowFlyout(sender);
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowFlyout(sender);
        }

        private static void ShowFlyout(object sender)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private void MenuFlyoutItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var m = sender as MenuFlyoutItem;
            var d = m.DataContext as Duan;

            string copied_content = $"转自煎蛋网:\n作者:{d.Author}\nID:{d.DuanID}\n{d.Content}\n(jandan.net | 地球没有新鲜事)";
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(copied_content);
            Clipboard.SetContent(dataPackage);
        }

        #region OOXX功能
        private void DuanVotePositiveIcon_Click(object sender, RoutedEventArgs e)
        {
            DuanVote(sender, true);
        }

        private void DuanVoteNegativeIcon_Click(object sender, RoutedEventArgs e)
        {
            DuanVote(sender, false);
        }

        private async void DuanVote(object sender, bool isLike)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;
            var c = b.Parent as RelativePanel;

            var msg = await _viewModel.Vote(duan, true);

            if (msg == null)
            {
                await PopupMessage("网络不好，请稍后重试", 90, 2000);

                return;
            }

            if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    var t = c.Children[1] as TextBlock;
                    t.Text = (duan.VotePositive++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b1 = c.Children[0] as Button;
                    b1.Foreground = new SolidColorBrush(Colors.Red);

                    await PopupMessage("感谢您的OO！", 64, 2000);
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (duan.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    await PopupMessage("感谢您的XX！", 64, 2000);
                }
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                await PopupMessage("您已投过票了", 60, 2000);
            }
        }
        #endregion

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BoringGridView.Visibility == Visibility.Collapsed)
            {
                return;
            }

            double margin = BoringGridView.Padding.Left + BoringGridView.Padding.Right;

            double currentWidth = this.ActualWidth - margin;
            int columnCount = (int)Math.Floor(currentWidth / 230);
            double setWidth = (currentWidth - (columnCount - 1) * 10) / columnCount;

            var s = BoringGridView.ItemContainerStyle;
            Setter s4 = new Setter(GridViewItem.WidthProperty, setWidth);

            Style s_new = new Style(typeof(GridViewItem));
            s_new.Setters.Add(s4);

            BoringGridView.ItemContainerStyle = s_new;
        }

        private void DuanViewComment_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;

            _dViewModel.Update(bp.PicID);
            DuanSplitView.IsPaneOpen = true;

            //////////////////////////////////////////////
            CommentSubmitButton.Focus(FocusState.Pointer);
        }

        private void Duan_Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            _dViewModel.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;

            //////////////////////////////////////////////
            CommentSubmitButton.Focus(FocusState.Pointer);
        }

        private void BestCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(FreshDetailPage), new object[] { 1, e.ClickedItem as BestFreshComment });
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateHotPics();
            _viewModel.UpdateHotDuan();
            _viewModel.UpdateHotComm();
        }

        private void HotImagePositiveIcon_Click(object sender, RoutedEventArgs e)
        {
            HotImageVote(sender, true);
        }

        private void HotImageNegativeIcon_Click(object sender, RoutedEventArgs e)
        {
            HotImageVote(sender, false);
        }

        private async void HotImageVote(object sender, bool isLike)
        {
            var b = sender as Button;
            var boring = b.DataContext as BoringPic;
            var c = b.Parent as RelativePanel;

            var msg = await _viewModel.Vote(boring, true);

            if (msg == null)
            {
                await PopupMessage("网络不好，请稍后重试", 90, 2000);

                return;
            }

            if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    var t = c.Children[1] as TextBlock;
                    t.Text = (boring.VotePositive++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b1 = c.Children[0] as Button;
                    b1.Foreground = new SolidColorBrush(Colors.Red);

                    await PopupMessage("感谢您的OO！", 64, 2000);
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (boring.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    await PopupMessage("感谢您的XX！", 64, 2000);
                }
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                await PopupMessage("您已投过票了", 60, 2000);
            }
        }

        private void DuanCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var d = e.ClickedItem as DuanComment;
            var user_name = d.AuthorName;

            CommentInputTextBox.Text = $"@{user_name}: ";

            _dViewModel.ParentId = d.PostID;
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

        private async Task PopupMessage(string message, double textWidth, int disTime)
        {
            textBlockPopup.Text = message;
            popTipVote.HorizontalOffset = -textWidth;
            popTipVote.IsOpen = true;   // 提示再按一次
            await Task.Delay(disTime);  // 1000ms后关闭提示
            popTipVote.IsOpen = false;
        }

        private void Dia_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var csd = sender.Content as CommentSubmitDialogue;

            DataShareManager.Current.UserName = csd.UserName;
            DataShareManager.Current.EmailAdd = csd.Email;
        }

        private void DuanSplitView_PaneClosed(SplitView sender, object args)
        {
            CommentInputTextBox.Text = "";

            DataShareManager.Current.UserName = "";
            DataShareManager.Current.EmailAdd = "";
        }
    }
}
