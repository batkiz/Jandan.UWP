using Jandan.UWP.Models;
using Jandan.UWP.ViewModels;
using Jandan.Control;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DuanPage : Page
    {
        /// <summary>
        /// 段子的View Model
        /// </summary>
        DuanViewModel _viewModel;
        /// <summary>
        /// 段子评论的View Model
        /// </summary>
        DuanCommentViewModel _dViewModel;

        // 用于从其他页面返回时保持滚动条的位置
        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        private bool _isFirstTimeGotFocus = true;

        #region 基本功能
        public DuanPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new DuanViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
        }
        /// <summary>
        /// 从其他页面导航回到段子
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.DuanPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            base.OnNavigatedTo(e);
        }

        private void DuanListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DuanSplitView.IsPaneOpen = true;

            var d = e.ClickedItem as Duan;
            var commentId = d.DuanID;

            _dViewModel.Update(commentId);
        }

        private void DuanSplitView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DuanGridView.Visibility == Visibility.Collapsed)
            {
                return;
            }

            double margin = DuanGridView.Padding.Left + DuanGridView.Padding.Right;

            double currentWidth = this.ActualWidth - margin;
            int columnCount = (int)Math.Floor(currentWidth / 230);
            double setWidth = (currentWidth - (columnCount - 1) * 10) / columnCount;

            var s = DuanGridView.ItemContainerStyle;
            Setter s4 = new Setter(GridViewItem.WidthProperty, setWidth);
            Setter s3 = new Setter(GridViewItem.VerticalContentAlignmentProperty, VerticalAlignment.Top);

            Style s_new = new Style(typeof(GridViewItem));
            s_new.Setters.Add(s3);
            s_new.Setters.Add(s4);

            DuanGridView.ItemContainerStyle = s_new;
        }

        private void Page_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //double delta = e.Cumulative.Translation.X;

            //if (delta > 200)
            //{
            //    Frame.Navigate(typeof(FreshPage));
            //}
            //else if (delta < 200)
            //{
            //    Frame.Navigate(typeof(BoringPicsPage));
            //}
        }
        #endregion

        #region 处理滚动条位置保存
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_persistedPosition))
            {
                await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(this.DuanListView, _persistedPosition, this.GetItem);
            }
        }

        private IAsyncOperation<object> GetItem(string key)
        {
            if (_viewModel.Duans == null)
            {
                return null;
            }
            return Task.Run(() =>
            {
                if (_viewModel.Duans.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return (object)_viewModel.Duans.FirstOrDefault(i => i.DuanID == key);
                }
            }).AsAsyncOperation();
        }

        private string GetKey(object item)
        {
            var singleItem = item as Duan;
            if (singleItem != null)
            {
                _persistedItemContainerHeight = (DuanListView.ContainerFromItem(item) as ListViewItem).ActualHeight;
                _persistedItemKey = singleItem.DuanID;
                return _persistedItemKey;
            }
            else
            {
                return string.Empty;
            }
        }

        private void DuanListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var singleItem = args.Item as Duan;

            if (singleItem != null && singleItem.DuanID == _persistedItemKey)
            {
                if (!args.InRecycleQueue)
                {
                    args.ItemContainer.Height = _persistedItemContainerHeight;
                }
                else
                {
                    args.ItemContainer.ClearValue(HeightProperty);
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _persistedPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(DuanListView, GetKey);

            base.OnNavigatingFrom(e);
        }
        #endregion

        #region 刷新列表
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void pullToRefreshBar_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }
        #endregion

        #region 段子评论
        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var duan = b.DataContext as Duan;

            _dViewModel.Update(duan.DuanID);
            DuanSplitView.IsPaneOpen = true;

            //////////////////////////////////////////////
            CommentSubmitButton.Focus(FocusState.Pointer);
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }
        #endregion
        
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
                textBlockPopup.Text = "网络不好，请稍后重试";
                popTipVote.HorizontalOffset = -90;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;

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

                    textBlockPopup.Text = "感谢您的OO！";
                }
                else
                {
                    var t = c.Children[3] as TextBlock;
                    t.Text = (duan.VoteNegative++).ToString();
                    t.Foreground = new SolidColorBrush(Colors.Red);

                    var b2 = c.Children[2] as Button;
                    b2.Foreground = new SolidColorBrush(Colors.Red);

                    textBlockPopup.Text = "感谢您的XX！";
                }
                
                popTipVote.HorizontalOffset = -64;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;
            }
            else if (msg.Contains("YOU'VE VOTED"))
            {
                textBlockPopup.Text = "您已投过票了";
                popTipVote.HorizontalOffset = -60;
                popTipVote.IsOpen = true;   // 提示再按一次
                await Task.Delay(2000);  // 1000ms后关闭提示
                popTipVote.IsOpen = false;
            }
        }
        #endregion

        #region 段子复制
        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
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

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowFlyout(sender);
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
        #endregion

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
                if (JsonObject.TryParse(r,out j))
                {
                    textBlockPopup.Text = "评论成功！";
                    popTipVote.HorizontalOffset = -40;
                    popTipVote.IsOpen = true;   // 提示再按一次
                    await Task.Delay(2000);  // 1000ms后关闭提示
                    popTipVote.IsOpen = false;
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

        private void CommentInputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (_isFirstTimeGotFocus)
            //{
            //    _isFirstTimeGotFocus = false;
            //    CommentSubmitButton.Focus(FocusState.Pointer);
            //}
        }

        private void DuanSplitView_PaneClosed(SplitView sender, object args)
        {
            //_isFirstTimeGotFocus = true;
        }
    }
}
