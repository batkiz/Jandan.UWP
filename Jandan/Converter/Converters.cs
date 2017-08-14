using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

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
                    return (double)1;
                    
                default:
                case PageFontSize.Normal:
                    return (double)2;
                    
                case PageFontSize.Large:
                    return (double)3;                    
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var v = (double)value;

            switch (v)
            {
                case 1:
                    return PageFontSize.Small;

                default:
                case 2:
                    return PageFontSize.Normal;

                case 3:
                    return PageFontSize.Large;
            }
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
    /// <summary>
    /// 将Author对象转换为输出字符串格式
    /// </summary>
    public class AuthorToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (Authors)value;

            return a.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 将Tag列表转换为输出字符串格式
    /// </summary>
    public class TagToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var t = (List<Tags>)value;
            string s = "";
            t.ForEach((tag) => { s = s + '|' + tag.Title; });
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }

            return s.Remove(0, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 将发布时间转换为简易格式
    /// </summary>
    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var d = (string)value;
            DateTime dt = DateTime.Parse(d);

            DateTime now = DateTime.Now;
            var days_past = now.Date.Subtract(dt.Date).Days;

            string s = "";
            if (days_past < 7)
            {
                switch (days_past)
                {
                    case 0:
                        s = $"今天 · {dt.TimeOfDay.ToString(@"hh\:mm")}";
                        break;
                    case 1:
                        s = $"昨天 · {dt.TimeOfDay.ToString(@"hh\:mm")}";
                        break;
                    case 2:
                        s = $"前天 · {dt.TimeOfDay.ToString(@"hh\:mm")}";
                        break;
                    default:
                        s = $"{days_past}天前 · {dt.TimeOfDay.ToString(@"hh\:mm")}";
                        break;
                }
            }
            else if (days_past < 30)
            {
                s = $"{days_past / 7}周前 · {dt.ToString(@"MM-dd")}";
            }
            else if (days_past < 365)
            {
                s = $"{days_past / 30}月前 · {dt.ToString(@"MM-dd")}";
            }
            else
            {
                s = $"很久以前 · {dt.ToString(@"YY-MM-dd")}";
            }

            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class UrlToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return null;
            }

            else return new BitmapImage(new Uri(value as string, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
