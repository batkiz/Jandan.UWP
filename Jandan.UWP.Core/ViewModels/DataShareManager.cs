using Windows.UI.ViewManagement;

namespace Jandan.UWP.Core.ViewModels
{
    public enum PicDetailType { Boring, Hot, Meizi };
    public enum PageIndex
    {
        MainPage = -1,
        FreshPage = 1,
        DuanPage = 2,
        BoringPage = 3,
        HotPage = 4,
        FreshDetailPage = 5,
        PicDetailPage = 6,
        MeiziPage = 7,
        AboutPage = 8
    };

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

        public bool IsPicDetailPageSizeChanged { get; set; } = false;

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

        // 是否是移动端
        private bool _isMobile;
        public bool IsMobile { get { return _isMobile; } }

        // 是否显示NSFW图
        private bool _isShowNSFW;
        public bool IsShowNSFW { get { return _isShowNSFW; } }

        // 是否显示不受欢迎的图
        private bool _isShowUnwelcome;
        public bool IsShowUnwelcome { get { return _isShowUnwelcome; } }

        // 评论用户名
        public string UserName { get; set; }
        // 评论邮箱
        public string EmailAdd { get; set; }

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

            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (string.Equals(platformFamily, "Windows.Mobile"))
            {
                _isMobile = true;
            }
            else
            {
                _isMobile = false;
            }

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

            // NSFW = 0 No NSFW images | NSFW = 1 Show NSFW images
            if (roamingSettings.Values.ContainsKey("NSFW"))
            {
                _isShowNSFW = int.Parse(roamingSettings.Values["NSFW"].ToString()) == 0 ? false : true;
            }
            else
            {
                _isShowNSFW = false;
            }

            // UNWELCOME = 0 No Unwelcome images | UNWELCOME = 1 Show Unwelcome images
            if (roamingSettings.Values.ContainsKey("UNWELCOME"))
            {
                _isShowUnwelcome = int.Parse(roamingSettings.Values["UNWELCOME"].ToString()) == 0 ? false : true;
            }
            else
            {
                _isShowUnwelcome = false;
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

        public void UpdateNSFW(bool isNSFW)
        {
            _isShowNSFW = isNSFW ? true : false;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["NSFW"] = _isShowNSFW ? 1 : 0;
            OnShareDataChanged();
        }

        public void UpdateUnwelcome(bool isUnwelcome)
        {
            _isShowUnwelcome = isUnwelcome ? true : false;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["UNWELCOME"] = _isShowUnwelcome ? 1 : 0;
            OnShareDataChanged();
        }

        public delegate void ShareDataChangedEventHandler();
    }
}
