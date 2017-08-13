using System;
using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.ViewModels;
using Windows.UI.Xaml;

namespace Jandan.UWP.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ElementTheme _appTheme;
        public ElementTheme AppTheme
        {
            get { return _appTheme; }
            set { _appTheme = value; OnPropertyChanged(); }
        }

        private bool _is_loading;
        public bool IsLoading
        {
            get { return _is_loading; }
            set { _is_loading = value; OnPropertyChanged(); }
        } 

        public MainViewModel()
        {
            Update();

            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        private void Update()
        {
            IsLoading = true;


            AppTheme = DataShareManager.Current.AppTheme;

            IsLoading = false;
        }

        private void Current_ShareDataChanged()
        {
            AppTheme = DataShareManager.Current.AppTheme;
        }

        public void ToggleAPPTheme()
        {
            DataShareManager.Current.ToggleAPPTheme();
        }
    }
}
