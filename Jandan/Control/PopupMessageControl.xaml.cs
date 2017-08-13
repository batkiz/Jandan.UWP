using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class PopupMessageControl : UserControl
    {
        private Popup m_Popup;

        public PopupMessageControl()
        {
            this.InitializeComponent();

            m_Popup = new Popup();
            this.Width = Window.Current.Bounds.Width;
            //this.Height = Window.Current.Bounds.Height;
            m_Popup.Child = this;

            //this.Loaded += NotifyPopup_Loaded;
            //this.Unloaded += NotifyPopup_Unloaded;
        }

        //private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    Window.Current.SizeChanged -= Current_SizeChanged;
        //}

        //private void NotifyPopup_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Window.Current.SizeChanged += Current_SizeChanged;
        //}

        //private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        //{
        //    this.Width = e.Size.Width;
        //    this.Height = e.Size.Height;
        //}

        public async void ShowAsync(string msg, int ms = 1000)
        {
            PopupInfo.Text = msg;

            this.m_Popup.IsOpen = true;
            await Task.Delay(ms);
            this.m_Popup.IsOpen = false;
        }
    }
}
