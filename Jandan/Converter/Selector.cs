using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Jandan.UWP.UI
{
    public sealed class CustomDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DuanTextTemplate { get; set; }
        public DataTemplate DuanImageTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Duan list = item as Duan;
            if (list != null)
            {
                switch (list.ContentType)
                {
                    default:
                    case "text":
                        return DuanTextTemplate;
                    case "image":
                        return DuanImageTemplate;
                }
            }
            return null;
        }
    }

    public sealed class CommentDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentTemplate { get; set; }
        public DataTemplate CommentWithImageTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Tucao list = item as Tucao;
            if (list != null)
            {
                switch (list.ContentType.ToLower())
                {
                    default:
                    case "text":
                        return CommentTemplate;
                    case "text_with_image":
                        return CommentWithImageTemplate;
                }
            }
            return null;
        }
    }
}
