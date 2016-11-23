using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jandan.UWP.Core.ViewModels
{
    public class SettingViewModel :ViewModelBase
    {
        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get { return _isDarkMode; }
            set { _isDarkMode = value; OnPropertyChanged(); }
        }

        private bool _isNoImageMode;
        public bool IsNoImageMode
        {
            get { return _isNoImageMode; }
            set { _isNoImageMode = value;OnPropertyChanged(); }
        }

        public SettingViewModel()
        {
            Update();

            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        private void Update()
        {
            IsDarkMode = DataShareManager.Current.AppTheme == Windows.UI.Xaml.ElementTheme.Dark ? true : false;
            IsNoImageMode = DataShareManager.Current.isNoImageMode ? true : false;
        }

        private void Current_ShareDataChanged()
        {
            IsDarkMode = DataShareManager.Current.AppTheme == Windows.UI.Xaml.ElementTheme.Dark ? true : false;
            IsNoImageMode = DataShareManager.Current.isNoImageMode ? true : false;
        }

        public void ExchangeDarkMode(bool isDark)
        {
            DataShareManager.Current.UpdateAPPTheme(isDark);
        }

        public void ExchangeNoImageMode(bool isNoImg)
        {
            DataShareManager.Current.UpdateNoImage(isNoImg);
        }
    }
}
