using Jandan.UWP.Core.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Jandan.UWP.Core.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get { return _isDarkMode; }
            set { Set(ref _isDarkMode, value); }
        }

        private bool _isAutoDarkMode;
        public bool IsAutoDarkMode
        {
            get { return _isAutoDarkMode; }
            set { Set(ref _isAutoDarkMode, value); }
        }

        private bool _isNoImageMode;
        public bool IsNoImageMode
        {
            get { return _isNoImageMode; }
            set { Set(ref _isNoImageMode, value); }
        }

        private TimeSpan _startTime;
        public TimeSpan StartTime
        {
            get { return _startTime; }
            set { Set(ref _startTime, value); }
        }

        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get { return _endTime; }
            set { Set(ref _endTime, value); }
        }

        private string _id;
        public string ID
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { Set(ref _email, value); }
        }

        private string _thirdpartyname;
        public string ThirdPartyName
        {
            get { return _thirdpartyname; }
            set { Set(ref _thirdpartyname, value); }
        }

        public string IdandEmail
        {
            get { return $"用户名:{ID}     邮箱:{Email}"; }
        }

        public SettingViewModel()
        {
            Update();

            DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        public void Update()
        {
            IsDarkMode = DataShareManager.Current.AppTheme == Windows.UI.Xaml.ElementTheme.Dark ? true : false;

            IsNoImageMode = DataShareManager.Current.IsNoImageMode ? true : false;

            IsAutoDarkMode = DataShareManager.Current.isAutoDarkMode ? true : false;
            StartTime = DataShareManager.Current.StartTime;
            EndTime = DataShareManager.Current.EndTime;

            ID = DataShareManager.Current.UserName;
            Email = DataShareManager.Current.EmailAdd;

            ThirdPartyName = DataShareManager.Current.ThirdPartyUserName;
        }

        private void Current_ShareDataChanged()
        {
            IsDarkMode = DataShareManager.Current.AppTheme == Windows.UI.Xaml.ElementTheme.Dark ? true : false;

            IsNoImageMode = DataShareManager.Current.IsNoImageMode ? true : false;

            IsAutoDarkMode = DataShareManager.Current.isAutoDarkMode ? true : false;
            StartTime = DataShareManager.Current.StartTime;
            EndTime = DataShareManager.Current.EndTime;

            ID = DataShareManager.Current.UserName;
            Email = DataShareManager.Current.EmailAdd;

            ThirdPartyName = DataShareManager.Current.ThirdPartyUserName;
        }

        public void ExchangeDarkMode(bool isDark)
        {
            DataShareManager.Current.UpdateAPPTheme(isDark);
        }

        public void ExchangeNoImageMode(bool isNoImg)
        {
            DataShareManager.Current.UpdateNoImage(isNoImg);
        }

        public void ExchangeAutoDarkMode(bool isAuto)
        {
            DataShareManager.Current.UpdateAutoDarkMode(isAuto);
        }

        private void Printlog(string info)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " " + info);
#endif
        }
    }
}
