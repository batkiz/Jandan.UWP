using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.UI
{
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
