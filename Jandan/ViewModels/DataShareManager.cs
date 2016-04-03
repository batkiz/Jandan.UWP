using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Jandan.UWP.ViewModels
{
    public sealed class DataShareManager
    {
        // 新鲜事的页数
        public int FreshNewsPage { get; set; }
        // 段子的页数
        public int DuanItemPage { get; set; }
        // 无聊图的页数
        public int BoringItemPage { get; set; }
        // 妹子图的页数
        public int MeiziItemPage { get; set; }

        // 当前页面索引值
        private PageIndex _previousPageIndex = PageIndex.MainPage;
        private PageIndex _currentPageIndex = PageIndex.MainPage;
        public PageIndex PreviousPageIndex
        {
            get
            {
                return _previousPageIndex;
            }
        }
        public PageIndex CurrentPageIndex
        {
            get
            {
                return _currentPageIndex;
            }
            set
            {
                _previousPageIndex = _currentPageIndex;
                _currentPageIndex = value;
            }
        }

        // 主题模式(夜间模式或日间模式)        
        private bool _isDarkMode;
        public bool IsDarkMode { get { return _isDarkMode; } }

        private static DataShareManager _current;
        public static DataShareManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DataShareManager();
                }
                return _current;
            }
        }

        public event ShareDataChangedEventHandler ShareDataChanged;

        public DataShareManager()
        {
            FreshNewsPage = 2;
            DuanItemPage = 2;
            BoringItemPage = 2;
            MeiziItemPage = 2;

            CurrentPageIndex = PageIndex.MainPage;
            
            LoadData();
        }

        private void LoadData()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            // APP_THEME = 0 Light mode | APP_THEME = 1 Dark mode
            if (roamingSettings.Values.ContainsKey("APP_THEME"))
            {
                _isDarkMode = int.Parse(roamingSettings.Values["APP_THEME"].ToString()) == 0 ? false : true;
            }
            else
            {
                _isDarkMode = false;
            }
        }

        private void OnShareDataChanged()
        {
            if (ShareDataChanged != null)
            {
                ShareDataChanged();
            }
        }

        public void UpdateAPPTheme(bool isDark)
        {
            _isDarkMode = isDark ? true : false;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["APP_THEME"] = _isDarkMode ? 1 : 0;
            OnShareDataChanged();
        }

        public delegate void ShareDataChangedEventHandler();
    }
}
