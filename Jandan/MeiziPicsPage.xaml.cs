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
using Jandan.UWP.Models;
using Jandan.UWP.ViewModels;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MeiziPicsPage : Page
    {
        MeiziViewModel _viewModel;
        DuanCommentViewModel _dViewModel;

        private static double _persistedItemContainerHeight = -1;
        private static string _persistedItemKey = "";
        private static string _persistedPosition = "";

        private bool just_returned = false;

        public MeiziPicsPage()
        {
            this.InitializeComponent();

            DataContext = _viewModel = new MeiziViewModel();
            DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_persistedPosition))
            {
                await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(this.BoringListView, _persistedPosition, this.GetItem);
                BoringGridView.ScrollIntoView(this.GetItem(_persistedItemKey));
            }
        }

        private IAsyncOperation<object> GetItem(string key)
        {
            if (_viewModel.Meizi == null)
            {
                return null;
            }
            return Task.Run(() =>
            {
                if (_viewModel.Meizi.Count <= 0)
                {
                    return null;
                }
                else
                {
                    return (object)_viewModel.Meizi.FirstOrDefault(i => i.PicID == key);
                }
            }).AsAsyncOperation();
        }

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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (BoringGridView.Visibility == Visibility.Collapsed)
            {
                _persistedPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(BoringListView, GetKey);
            }
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataShareManager.Current.CurrentPageIndex = PageIndex.MeiziPage;

            if (e.NavigationMode == NavigationMode.Back)
            {
                just_returned = true;
                return;
            }
            base.OnNavigatedTo(e);
            //DataContext = _viewModel = new MeiziViewModel();
            //DuanCommentListView.DataContext = _dViewModel = new DuanCommentViewModel();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void ListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (string.Equals(platformFamily, "Windows.Mobile"))
            {
                var li = sender as ListView;
                this.Frame.Navigate(typeof(PicDetailPage), new object[] { li.DataContext as BoringPic });
            }
        }

        private void DuanSplitView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            DuanSplitView.IsPaneOpen = false;
        }

        private void Tucao_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var bp = b.DataContext as BoringPic;

            _dViewModel.Update(bp.PicID);

            DuanSplitView.IsPaneOpen = true;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BoringGridView.Visibility == Visibility.Collapsed)
            {
                return;
            }

            if (just_returned)
            {
                just_returned = false;
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

        private void pullToRefreshBar_RefreshInvoked(DependencyObject sender, object args)
        {
            RefreshPage();
        }

        private void BoringListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(PicDetailPage), new object[] { e.ClickedItem as BoringPic, PicDetailType.Meizi, _viewModel.Meizi });
        }


    }
}
