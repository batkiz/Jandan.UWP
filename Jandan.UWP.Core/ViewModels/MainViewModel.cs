using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;
using Windows.ApplicationModel.Background;

namespace Jandan.UWP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        APIService _api = new APIService();

        //private bool _isDarkMode;
        //public bool IsDarkMode
        //{
        //    get { return _isDarkMode; }
        //    set { _isDarkMode = value; OnPropertyChanged(); }
        //}

        private bool _is_loading;
        public bool IsLoading
        {
            get { return _is_loading; }
            set { _is_loading = value; OnPropertyChanged(); }
        } 

        public MainViewModel()
        {
            ;
        }
    }
}
