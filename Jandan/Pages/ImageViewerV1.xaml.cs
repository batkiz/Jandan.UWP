using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Jandan.UWP.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageViewerV1 : Page
    {
        PicDetailViewModel _viewModel;

        object ItemList;
        BoringPic CurrentItem;
        PicDetailType DetailType;

        public ImageViewerV1()
        {
            this.InitializeComponent();
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

                CommentControl.Update(p.PicID);

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

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PicDownload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    
}
