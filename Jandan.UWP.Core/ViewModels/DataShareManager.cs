using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
        AboutPage = 8,
        SettingPage = 9
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
        public ElementTheme AppTheme { get; private set; }

       
        // 是否省流模式
        public bool isNoImageMode { get; private set; }

        // 是否是移动端
        public bool IsMobile { get; private set; }

        // 是否显示NSFW图
        public bool IsShowNSFW { get; private set; }

        // 是否显示不受欢迎的图
        public bool IsShowUnwelcome { get; private set; }

        // 评论用户名
        public string UserName { get; set; }
        // 评论邮箱
        public string EmailAdd { get; set; }

        public bool isCortanaRegistered { get; private set; }
        public bool isLiveTileRegistered { get; private set; }

        private static DataShareManager _current;
        public static DataShareManager Current
        {
            get
            {
                if (_current == null) _current = new DataShareManager();
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
                IsMobile = true;
            }
            else
            {
                IsMobile = false;
            }

            LoadData();

            //var screen_info = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
        }

        private void LoadData()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            // APP_THEME = 0 Light mode | APP_THEME = 1 Dark mode
            if (roamingSettings.Values.ContainsKey("APP_THEME"))
            {
                AppTheme = int.Parse(roamingSettings.Values["APP_THEME"].ToString()) == 0 ? ElementTheme.Light : ElementTheme.Dark;
            }
            else
            {
                AppTheme = ElementTheme.Light;
            }
            if (roamingSettings.Values.ContainsKey("NO_IMAGE_MODE"))
            {
                isNoImageMode = int.Parse(roamingSettings.Values["NO_IMAGE_MODE"].ToString()) == 0 ? true : false;
            }
            else
            {
                isNoImageMode = false;
            }

            // NSFW = 0 No NSFW images | NSFW = 1 Show NSFW images
            if (roamingSettings.Values.ContainsKey("NSFW"))
            {
                IsShowNSFW = int.Parse(roamingSettings.Values["NSFW"].ToString()) == 0 ? false : true;
            }
            else
            {
                IsShowNSFW = false;
            }

            // UNWELCOME = 0 No Unwelcome images | UNWELCOME = 1 Show Unwelcome images
            if (roamingSettings.Values.ContainsKey("UNWELCOME"))
            {
                IsShowUnwelcome = int.Parse(roamingSettings.Values["UNWELCOME"].ToString()) == 0 ? false : true;
            }
            else
            {
                IsShowUnwelcome = false;
            }

            // CORTANA = 0 Not Registered | CORTANA = 1 Registered
            if (localSettings.Values.ContainsKey("CORTANA"))
            {
                isCortanaRegistered = int.Parse(localSettings.Values["CORTANA"].ToString()) == 0 ? false : true;
            }
            else
            {
                isCortanaRegistered = false;
            }

            // LIVETILE = 0 Not Registered | LIVETILE = 1 Registered
            if (localSettings.Values.ContainsKey("LIVETILE"))
            {
                isLiveTileRegistered = int.Parse(localSettings.Values["LIVETILE"].ToString()) == 0 ? false : true;
            }
            else
            {
                isLiveTileRegistered = false;
            }
        }

        private void OnShareDataChanged()
        {
            ShareDataChanged?.Invoke();
        }

        internal void UpdateNoImage(bool isNoImg)
        {
            isNoImageMode = isNoImg;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["NO_IMAGE_MODE"] = isNoImageMode ? 0 : 1;
            OnShareDataChanged();
        }

        public void UpdateAPPTheme(bool isDark)
        {
            AppTheme = isDark ? ElementTheme.Dark : ElementTheme.Light;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["APP_THEME"] = AppTheme==ElementTheme.Light ? 0 : 1;
            OnShareDataChanged();
        }

        public void ToggleAPPTheme()
        {
            AppTheme = AppTheme==ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["APP_THEME"] = AppTheme == ElementTheme.Light ? 0 : 1;
            OnShareDataChanged();
        }

        public void UpdateNSFW(bool isNSFW)
        {
            IsShowNSFW = isNSFW;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["NSFW"] = IsShowNSFW ? 1 : 0;
            OnShareDataChanged();
        }

        public void UpdateUnwelcome(bool isUnwelcome)
        {
            IsShowUnwelcome = isUnwelcome;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["UNWELCOME"] = IsShowUnwelcome ? 1 : 0;
            OnShareDataChanged();
        }

        public void EnableCortana()
        {
            isCortanaRegistered = true;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["CORTANA"] = 1;
            OnShareDataChanged();
        }

        public void EnableLiveTile()
        {
            isLiveTileRegistered = true;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["LIVETILE"] = 1;
            OnShareDataChanged();
        }

        public delegate void ShareDataChangedEventHandler();
    }
}
