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

namespace Jandan.UWP.Control
{
    public class NewsContentPanel : Panel
    {
        public double ColumnContentWidth
        {
            get { return (double)GetValue(ColumnContentWidthProperty); }
            set { SetValue(ColumnContentWidthProperty, value); }
        }
        public static readonly DependencyProperty ColumnContentWidthProperty =
            DependencyProperty.Register(
                nameof(ColumnContentWidth),
                typeof(double),
                typeof(NewsContentPanel),
                new PropertyMetadata(0, RequestRefreshLayout));

        public string RawContent
        {
            get { return (string)GetValue(RawContentProperty); }
            set { SetValue(RawContentProperty, value); }
        }
        public readonly DependencyProperty RawContentProperty =
            DependencyProperty.Register(
                nameof(RawContent),
                typeof(string),
                typeof(NewsContentPanel),
                new PropertyMetadata("", RequestRefreshLayout));

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        private static void RequestRefreshLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NewsContentPanel).InvalidateMeasure();
            (d as NewsContentPanel).InvalidateArrange();
        }


    }
}
