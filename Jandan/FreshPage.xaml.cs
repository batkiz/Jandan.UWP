using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Jandan.UWP.Tools;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Jandan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FreshPage : Page
    {
        FreshViewModel _viewModel;

        private bool just_returned = false;

        public FreshPage()
        {
            this.InitializeComponent();

            DispatcherManager.Current.Dispatcher = Dispatcher;
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
            this.DataContext = _viewModel = new FreshViewModel();
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
            this.Frame.Navigate(typeof(FreshDetailPage), new object[] { e.ClickedItem as Fresh });
        }

        public void RefreshPage()
        {
            _viewModel.Update();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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
            Setter s3 = new Setter(GridViewItem.VerticalContentAlignmentProperty, VerticalAlignment.Top);
            Setter s4 = new Setter(GridViewItem.WidthProperty, setWidth);
            Setter s5 = new Setter(GridViewItem.HeightProperty, 240);

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
