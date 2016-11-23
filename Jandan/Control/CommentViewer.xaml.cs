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
using Jandan.UWP.ViewModels;
using Jandan.UWP.Core.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Jandan.UWP.Control
{
    public sealed partial class CommentViewer : UserControl
    {
        public static readonly DependencyProperty DViewModelProperty = DependencyProperty.Register(
            nameof(DViewModel),
            typeof(UserCommentViewModel),
            typeof(CommentViewer),
            new PropertyMetadata(null));

        public UserCommentViewModel DViewModel
        {
            get { return (UserCommentViewModel)GetValue(DViewModelProperty); }
            set { SetValue(DViewModelProperty, value); }
        }

        public CommentViewer()
        {
            this.InitializeComponent();

            DViewModel = new UserCommentViewModel()
            {
                UserComments = new CollectionViewSource()
                {
                    IsSourceGrouped = true,
                },
                ParentId = string.Empty,
                PostId = string.Empty,
                IsLoadingComments = false,
                ResponseComment = string.Empty
            };
        }

        private void DuanCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void CommentSubmitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class UriToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var uri = value as Uri;

            BitmapImage image = new BitmapImage();
            image.UriSource = uri;

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var image = value as BitmapImage;

            return image.UriSource;
        }
    }
}
