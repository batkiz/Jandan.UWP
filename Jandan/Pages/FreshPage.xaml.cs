using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan.UWP.UI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FreshPage : Page
    {
        FreshViewModel _viewModel;

        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        private bool just_returned = false;

        public FreshPage()
        {
            InitializeComponent();

            DataContext = _viewModel = new FreshViewModel();

            DispatcherManager.Current.Dispatcher = Dispatcher;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_persistedPosition))
            {
                await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(this.FreshListView, _persistedPosition, this.GetItem);
            }
        }

        private IAsyncOperation<object> GetItem(string key)
        {
            if (_viewModel.News == null)
            {
                return null;
            }
            return Task.Run(() =>
            {
                if (_viewModel.News.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return (object)_viewModel.News.FirstOrDefault(i => i.ID == key);
                }
            }).AsAsyncOperation();
        }

        private string GetKey(object item)
        {
            var singleItem = item as Fresh;
            if (singleItem != null)
            {
                _persistedItemContainerHeight = (FreshListView.ContainerFromItem(item) as ListViewItem).ActualHeight;
                _persistedItemKey = singleItem.ID;
                return _persistedItemKey;
            }
            else
            {
                return string.Empty;
            }
        }

        private void FreshListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var singleItem = args.Item as Fresh;

            if (singleItem != null && singleItem.ID == _persistedItemKey)
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
            if (FreshGridView.Visibility == Visibility.Collapsed)
            {
                _persistedPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(FreshListView, GetKey);
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.FreshPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                just_returned = true;
                return;
            }
            base.OnNavigatedTo(e);
            //DataContext = _viewModel = new FreshViewModel();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void FreshListView_ItemClick(object sender, ItemClickEventArgs e)
        {            
            Frame.Navigate(typeof(FreshDetailPage), new object[] { 0, e.ClickedItem as Fresh });
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (FreshGridView.Visibility == Visibility.Collapsed)
            {
                return;
            }

            if (just_returned)
            {
                just_returned = false;
                return;
            }

            double margin = FreshGridView.Padding.Left + FreshGridView.Padding.Right;

            double currentWidth = this.ActualWidth - margin;
            int columnCount = (int)Math.Floor(currentWidth / 230);
            double setWidth = (currentWidth - (columnCount - 1) * 10) / columnCount;

            var s = FreshGridView.ItemContainerStyle;
            //Setter s1 = new Setter(GridViewItem.MarginProperty, new Thickness(4));
            //Setter s2 = new Setter(GridViewItem.PaddingProperty, new Thickness(5));
            Setter s3 = new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Top);
            Setter s4 = new Setter(WidthProperty, setWidth);
            Setter s5 = new Setter(HeightProperty, 240);

            Style s_new = new Style(typeof(GridViewItem));
            //s_new.Setters.Add(s1);
            //s_new.Setters.Add(s2);
            s_new.Setters.Add(s3);
            s_new.Setters.Add(s4);
            s_new.Setters.Add(s5);

            FreshGridView.ItemContainerStyle = s_new;
        }

        private void pullToRefreshBar_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }
    }
}
