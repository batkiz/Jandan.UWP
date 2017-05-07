using Jandan.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.UI
{
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
        {switch (value.ToString())
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
