using Jandan.UWP.Control;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan.UWP.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BoringPicsPage : Page
    {
        /// <summary>
        /// 无聊图的View Model
        /// </summary>
        BoringViewModel _viewModel;
        /// <summary>
        /// 无聊图评论的View Model
        /// </summary>
        DuanCommentViewModel _dViewModel;

        // 用于从其他页面返回时保持滚动条的位置
        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        /// <summary>
        /// 用于进入妹子图的计数
        /// </summary>
        private int secret_count;
        /// <summary>
        /// 在页面尺寸缩放后根据此变量决定是否需要刷新
        /// </summary>
        private bool just_returned = false;

        #region 基本功能
        /// <summary>
        /// 构造函数，初始化列表数据
        /// </summary>
        public BoringPicsPage()
        {
            this.InitializeComponent();

            this.DataContext = _viewModel = new BoringViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
            LoadingCommentProgressBar.DataContext = _dViewModel;
        }
        /// <summary>
        /// 从其他页面导航回到无聊图
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.BoringPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                just_returned = true;
                return;
            }
            // 重置妹子图点击计数
            secret_count = 0;

            base.OnNavigatedTo(e);
        }
        /// <summary>
        /// 点击无聊图查看详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoringListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic, PicDetailType.Boring, _viewModel.Boring });
        }

        /// <summary>
        /// 页面大小改变时的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (just_returned)
            {
                just_returned = false;
                return;
            }

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
        #endregion

        #region 处理滚动条位置保存
        /// <summary>
        /// 打开无聊图页面，主要处理返回时的滚动条位置问题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_persistedPosition))
            {
                await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(this.BoringListView, _persistedPosition, this.GetItem);
                BoringGridView.ScrollIntoView(this.GetItem(_persistedItemKey));
            }
        }
        /// <summary>
        /// 通过Key定位列表中的Item
        /// </summary>
        /// <param name="key">图片ID</param>
        /// <returns></returns>
        private IAsyncOperation<object> GetItem(string key)
        {
            if (_viewModel.Boring == null)
            {
                return null;
            }
            return Task.Run(() =>
            {
                if (_viewModel.Boring.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return (object)_viewModel.Boring.FirstOrDefault(i => i.PicID == key);
                }
            }).AsAsyncOperation();
        }
        /// <summary>
        /// 通过列表中的Item得到对应的Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetKey(object item)
        {
            var singleItem = item as BoringPic;
            if (singleItem != null)
            {
                _persistedItemContainerHeight = (BoringListView.ContainerFromItem(item) as ListViewItem).ActualHeight;
                _persistedItemKey = singleItem.PicID;
                return _persistedItemKey;
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 列表内容改变时滚动条位置处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BoringListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var singleItem = args.Item as BoringPic;

            if (singleItem != null && singleItem.PicID == _persistedItemKey)
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
        /// <summary>
        /// 导航至其他页面之前，记下当前滚动条位置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (BoringGridView.Visibility == Visibility.Collapsed)
            {
                _persistedPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(BoringListView, GetKey);
            }            
            base.OnNavigatingFrom(e);
        }
        #endregion
        
        #region 刷新列表
        /// <summary>
        /// 点击刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        /// <summary>
        /// 刷新无聊图列表内容
        /// </summary>
        public void RefreshPage()
        {
            _viewModel.UpdateBoringPics();
        }

        /// <summary>
        /// 下拉刷新（未实现）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void pullToRefreshBar_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }
        #endregion

        #region 无聊图评论
        /// <summary>
        /// 双击评论列表关闭吐槽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }
        /// <summary>
        /// 单击吐槽按钮打开评论列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;

            _dViewModel.Update(bp.PicID);

            DuanSplitView.IsPaneOpen = true;

            //////////////////////////////////////////////
            CommentSubmitButton.Focus(FocusState.Pointer);
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
            var boring = b.DataContext as BoringPic;
            var c = b.Parent as RelativePanel;

            var msg = await _viewModel.Vote(boring, isLike);

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
        #endregion

        #region 隐藏功能
        /// <summary>
        /// 点击无聊图标题5次进入妹子图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            secret_count++;

            if (secret_count == 5)
            {
                this.Frame.Navigate(typeof(MeiziPicsPage));
                secret_count = 0;
                return;
            }

            string tips = $"再点击{5 - secret_count}次进入妹子图";
            
            await PopupMessage(tips, 75, 2000);
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
                if (JsonObject.TryParse(r, out j))
                {
                    await PopupMessage("评论成功！", 40, 2000);
                }

                CommentInputTextBox.Text = "";
            }

        }

        private async Task PopupMessage(string message, double textWidth, int disTime)
        {
            textBlockMeiziCount.Text = message;
            popTipsMeizi.HorizontalOffset = -textWidth;
            popTipsMeizi.IsOpen = true;   // 提示再按一次
            await Task.Delay(disTime);  // 1000ms后关闭提示
            popTipsMeizi.IsOpen = false;
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
