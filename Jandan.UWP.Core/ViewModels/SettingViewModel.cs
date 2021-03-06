﻿using Jandan.UWP.Core.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Jandan.UWP.Core.Style;

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

        private double _font_size;
        public double FontSize
        {
            get { return _font_size; }
            set
            {
                Set(ref _font_size, value);
                DataShareManager.Current.UpdateFontSize(FontSize);
            }
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

            _font_size = DataShareManager.Current.FontSize / 10.0;
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

            _font_size = DataShareManager.Current.FontSize / 10.0;
        }        

        public double UpdateFontSize(double scalar)
        {
            var cfs = 16 *  scalar;
            var ccfs = 14 * scalar;
            var cifs = 12 * scalar;

            if (Application.Current.Resources["ContentFontStyle"] is FontStyle f1) f1.FontSize = cfs;
            if (Application.Current.Resources["CommentContentFontStyle"] is FontStyle f2) f2.FontSize = ccfs;
            if (Application.Current.Resources["ContentInfoFontStyle"] is FontStyle f3) f3.FontSize = cifs;
            if (Application.Current.Resources["CommentContentInfoFontStyle"] is FontStyle f4) f4.FontSize = cifs;

            return scalar;
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
