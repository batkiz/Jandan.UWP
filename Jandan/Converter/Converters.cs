using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.UI
{
    public class IntToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int is_jandan_user = (int)value;

            switch (is_jandan_user)
            {
                case 0:
                    if (DataShareManager.Current.AppTheme== Windows.UI.Xaml.ElementTheme.Dark)
                    {
                        return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White);
                    }
                    else
                    {
                        return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black);
                    }                    
                case 1: // 打赏橙名
                    return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.OrangeRed);
                case 10: // 小编蓝名
                    return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Blue);
                case 11: // 打赏小编。。。
                    return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
                default:
                    return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 将PageFontSize枚举值转换成整数
    /// </summary>
    public class FontSizeToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (PageFontSize)value;

            switch (v)
            {
                case PageFontSize.Small:
                    return (double)8;
                    
                default:
                case PageFontSize.Normal:
                    return (double)16;
                    
                case PageFontSize.Large:
                    return (double)20;                    
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 将URL转换成图像
    /// </summary>
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = value as List<ImageItem>;

            if (v.Count > 0)
            {
                return v[0].URL;
            }
            else
            {
                return "ms-appx:///Assets/ImageNotAvailable.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 将布尔值转换为收藏图标（空心星星或实心星星）
    /// </summary>
    public class BooleanToFavStringConverter : IValueConverter
    {
        // 定义转换方法
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                default:
                case false:
                    return "\uE734";
                case true:
                    return "\uE735";
            }
        }

        // 定义反向转换方法
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch (value.ToString())
            {
                default:
                case "\uE734":
                    return false;
                case "\uE735":
                    return true;
            }            
        }
    }
}
