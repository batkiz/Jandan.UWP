using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Jandan.UWP.Core.Style
{
    public class FontStyle : DependencyObject
    {
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(FontStyle), new PropertyMetadata(16));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(FontStyle), new PropertyMetadata(new FontFamily("Segoe UI")));

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontSizeProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }
    }
}
