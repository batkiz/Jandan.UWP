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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Jandan.UWP.Control
{
    public sealed partial class PopupImageViewerControl : UserControl
    {
        private Popup m_Popup;

        private TransformGroup tfg = new TransformGroup();
        private ScaleTransform st = new ScaleTransform();
        private TranslateTransform tt = new TranslateTransform();

        public PopupImageViewerControl()
        {
            this.InitializeComponent();

            this.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
            this.ManipulationDelta += PopupImage_ManipulationDelta;
            this.PointerWheelChanged += PopImageViewer_PointerWheelChanged;
            tfg.Children.Add(st);
            tfg.Children.Add(tt);
            PopupImage.RenderTransform = tfg;

            m_Popup = new Popup();
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            m_Popup.Child = this;

            this.Loaded += NotifyPopup_Loaded;
            this.Unloaded += NotifyPopup_Unloaded;
        }

        private void PopImageViewer_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            st.CenterX = PopupGrid.ActualWidth / 2;
            st.CenterY = PopupGrid.ActualHeight / 2;

            var delta = e.GetCurrentPoint((UIElement)sender).Properties.MouseWheelDelta;

            if (delta > 0)
            {
                if (st.ScaleX < 6d)
                {
                    st.ScaleX += 0.5d;
                }
                if (st.ScaleY < 6d)
                {
                    st.ScaleY += 0.5d;
                }
            }
            else
            {
                if (st.ScaleX > 0.5d)
                {
                    st.ScaleX -= 0.5d;
                }
                if (st.ScaleY > 0.5d)
                {
                    st.ScaleY -= 0.5d;
                }
            }

            if (st.ScaleX == 0.5 && st.ScaleY == 0.5)
            {
                tt.X = tt.Y = 0;
            }

            if (st.ScaleX == 1 && st.ScaleY == 1)
            {
                this.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
            }
            else
            {
                this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.Scale | ManipulationModes.TranslateInertia;
            }
#if DEBUG
            PopupInfo.Text = $"x:{st.ScaleY}, y:{st.ScaleY}";
#endif
        }

        private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.Width = e.Size.Width;
            this.Height = e.Size.Height;
        }

        public void Show()
        {
            this.m_Popup.IsOpen = true;
        }

        public void Show(object obj)
        {
            PopupImage.Source = obj;

            this.m_Popup.IsOpen = true;
        }

        public void Show(object obj, string msg)
        {
            PopupImage.Source = obj;
            PopupInfo.Text = msg;

            this.m_Popup.IsOpen = true;
        }


        private void PopupImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.m_Popup.IsOpen = false;
        }

        private void PopupImage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (st.ScaleX == 1 && st.ScaleY == 1)
            {
                this.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
            }
            else if (st.ScaleX == 0.5 && st.ScaleY == 0.5)
            {
                this.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
            }
            else
            {
                this.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.Scale | ManipulationModes.TranslateInertia;
            }

            st.CenterX = PopupGrid.ActualWidth / 2;
            st.CenterY = PopupGrid.ActualHeight / 2;

            st.ScaleX *= e.Delta.Scale;
            st.ScaleY *= e.Delta.Scale;
            if (st.ScaleY < 0.5)
            {
                st.ScaleX = st.ScaleY = 0.5;
            }
            if (st.ScaleY > 6)
            {
                st.ScaleX = st.ScaleY = 6;
            }

            if (st.ScaleX == 0.5 && st.ScaleY == 0.5)
            {
                tt.X = tt.Y = 0;
            }
            else
            {
                tt.X += e.Delta.Translation.X;
                tt.Y += e.Delta.Translation.Y;
                StopWhenTranslateToEdge();
            }

#if DEBUG
            //PopupInfo.Text = $"x:{st.ScaleY}, y:{st.ScaleY}";
            double width = PopupImage.DesiredSize.Width * (st.ScaleX - 1) / 2;
            double height = PopupImage.DesiredSize.Height * (st.ScaleY - 1) / 2;
            PopupInfo.Text = $"x:{tt.X}, y:{tt.Y}, height:{height}, width:{width}";
#endif
        }

        private void StopWhenTranslateToEdge()
        {
            double factor = 1.0d;

            double width = PopupImage.DesiredSize.Width * (st.ScaleX - 1) / 2;
            if (tt.X > 0)
            {
                if (factor * width < tt.X)
                {
                    tt.X = factor * width;
                }
            }
            else if (tt.X < 0)
            {
                if (-factor * width > tt.X)
                {
                    tt.X = -factor * width;
                }
            }

            double height = PopupImage.DesiredSize.Height * (st.ScaleY - 1) / 2;
            if (factor * height < tt.Y)
            {
                tt.Y = factor * height;
            }
            else if (tt.Y < 0)
            {
                if (-factor * height > tt.Y)
                {
                    tt.Y = -factor * height;
                }
            }
        }

        private void PopupImage_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            //st.ScaleX = st.ScaleY = 1;
            //tt.X = tt.Y = 0;

            //this.ManipulationMode = ManipulationModes.System | ManipulationModes.Scale;
        }

        private void PopupGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.m_Popup.IsOpen = false;
        }
    }
}
