using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Jandan.UWP.Core.Style;
using Windows.Storage;
using System.Threading.Tasks;

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
        SettingPage = 9,
        FavouritePage = 10
    };
    public enum PageFontSize { Small = 1, Normal = 2, Large = 3};

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
        //private PageIndex _previousPageIndex = PageIndex.MainPage;
        public PageIndex PreviousPageIndex { get; private set; } = PageIndex.MainPage;
        private PageIndex _currentPageIndex = PageIndex.MainPage;
        public PageIndex CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set { PreviousPageIndex = _currentPageIndex; _currentPageIndex = value; }
        }

        // 字号大小
        //public PageFontSize FontSizes { get; private set; } = PageFontSize.Normal;
        public int FontSize { get; private set; } = 10;

        // 主题模式(夜间模式或日间模式)        
        public ElementTheme AppTheme { get; private set; }

       
        // 是否省流模式
        public bool IsNoImageMode { get; private set; }

        // 是否自动切换夜间模式
        public bool isAutoDarkMode { get; private set; }
        public TimeSpan StartTime { get; private set; } //= new TimeSpan(19, 30, 0);
        public TimeSpan EndTime { get; private set; } //= new TimeSpan(7, 30, 0);

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

        public bool IsCortanaRegistered { get; private set; }
        public bool IsLiveTileRegistered { get; private set; }

        // 本地存储的版本号,用于和关于信息中的版本号进行比较,如果不一致,则显示这一版本的更新内容
        public string StoredVersionNumber { get; private set; }

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

            CheckAppTheme();

            CheckUpdatedContentAsync();
        }

        // 判断本地存储版本号和关于信息中的当前版本号是否一致,不一致则显示更新内容,并将本地存储版本号更新为关于信息中的版本号
        private async void CheckUpdatedContentAsync()
        {
            //Jandan.UWP.Core.Models.About a = new Models.About();
            
            Version curr_version = new Version(Jandan.UWP.Core.Models.About.Current.VersionNumber);
            Version stor_version = new Version(StoredVersionNumber);

            if (curr_version != stor_version)
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Documents/updated.txt"));
                var msg = await FileIO.ReadTextAsync(file);
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(msg));

                XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Assets/Square150x150Logo.scale-200.png");
                ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "red graphic");

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);

                UpdateVersionNumber(curr_version.ToString());
            }            
        }

        // 检查夜间模式设置
        public void CheckAppTheme()
        {
            if (isAutoDarkMode)
            {
                var now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                // 若开始时间与结束时间相同, 出错, 自动重设结束时间
                if (TimeSpan.Compare(StartTime, EndTime) == 0)
                {
                    EndTime = StartTime.Add(new TimeSpan(0, 0, 1));
                    UpdateEndTime(EndTime);
                }

                if (TimeSpan.Compare(StartTime, EndTime) > 0)
                {
                    if (!((TimeSpan.Compare(now, EndTime) > 0) && (TimeSpan.Compare(StartTime, now) > 0)))
                    {
                        UpdateAPPTheme(true);
                    }
                    else
                    {
                        UpdateAPPTheme(false);
                    }
                }
                else
                {
                    if ((TimeSpan.Compare(now, EndTime) > 0) && (TimeSpan.Compare(StartTime, now) > 0))
                    {
                        UpdateAPPTheme(true);
                    }
                    else
                    {
                        UpdateAPPTheme(false);
                    }
                }
            }

            UpdateFontSize(FontSize/10.0);
        }

        // 从本地设置和漫游设置中读取设置参数
        private void LoadData()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            #region Theme-related
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
                IsNoImageMode = int.Parse(roamingSettings.Values["NO_IMAGE_MODE"].ToString()) == 0 ? true : false;
            }
            else
            {
                IsNoImageMode = false;
            }

            if (roamingSettings.Values.ContainsKey("AUTO_DARK_MODE"))
            {
                isAutoDarkMode = int.Parse(roamingSettings.Values["AUTO_DARK_MODE"].ToString()) == 0 ? true : false;
            }
            else
            {
                isAutoDarkMode = false;
            }
            if (localSettings.Values.ContainsKey("AUTO_DARK_START_TIME"))
            {
                StartTime = TimeSpan.Parse(localSettings.Values["AUTO_DARK_START_TIME"].ToString());
            }
            else
            {
                StartTime = new TimeSpan(19, 30, 0);
            }
            if (localSettings.Values.ContainsKey("AUTO_DARK_END_TIME"))
            {
                EndTime = TimeSpan.Parse(localSettings.Values["AUTO_DARK_END_TIME"].ToString());
            }
            else
            {
                EndTime = new TimeSpan(7, 30, 0);
            }
            #endregion
            #region Account-related
            // User Name and Email
            if (localSettings.Values.ContainsKey("USER_NAME"))
            {
                UserName = localSettings.Values["USER_NAME"].ToString();
            }
            else
            {
                UserName = "";
            }
            if (localSettings.Values.ContainsKey("EMAIL_ADDR"))
            {
                EmailAdd = localSettings.Values["EMAIL_ADDR"].ToString();
            }
            else
            {
                EmailAdd = "";
            }
            #endregion

            // Version Number Check
            if (localSettings.Values.ContainsKey("CURR_VERSION"))
            {
                StoredVersionNumber = localSettings.Values["CURR_VERSION"].ToString();
            }
            else
            {
                StoredVersionNumber = "0.0.0";
            }

            // Font Size
            if (localSettings.Values.ContainsKey("FONT_SIZE"))
            {
                FontSize = int.Parse(localSettings.Values["FONT_SIZE"].ToString());
            }
            else
            {
                FontSize = 10;
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
                IsCortanaRegistered = int.Parse(localSettings.Values["CORTANA"].ToString()) == 0 ? false : true;
            }
            else
            {
                IsCortanaRegistered = false;
            }

            // LIVETILE = 0 Not Registered | LIVETILE = 1 Registered
            if (localSettings.Values.ContainsKey("LIVETILE"))
            {
                IsLiveTileRegistered = int.Parse(localSettings.Values["LIVETILE"].ToString()) == 0 ? false : true;
            }
            else
            {
                IsLiveTileRegistered = false;
            }
        }

        private void OnShareDataChanged()
        {
            ShareDataChanged?.Invoke();
        }

        internal void UpdateNoImage(bool isNoImg)
        {
            IsNoImageMode = isNoImg;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["NO_IMAGE_MODE"] = IsNoImageMode ? 0 : 1;
            OnShareDataChanged();
        }

        public void UpdateFontSize(double p)
        {
            var cfs = 16 * p;
            var ccfs = 14 * p;
            var cifs = 12 * p;

            if (Application.Current.Resources["ContentFontStyle"] is FontStyle f1) f1.FontSize = cfs;
            if (Application.Current.Resources["CommentContentFontStyle"] is FontStyle f2) f2.FontSize = ccfs;
            if (Application.Current.Resources["ContentInfoFontStyle"] is FontStyle f3) f3.FontSize = cifs;
            if (Application.Current.Resources["CommentContentInfoFontStyle"] is FontStyle f4) f4.FontSize = cifs;

            FontSize = (int)(p*10);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["FONT_SIZE"] = FontSize;
            OnShareDataChanged();
        }

        public void UpdateVersionNumber(string v)
        {
            StoredVersionNumber = v;            
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["CURR_VERSION"] = StoredVersionNumber;
            OnShareDataChanged();
        }

        public void UpdateAutoDarkMode(bool isAuto)
        {
            isAutoDarkMode = isAuto;
            var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            roamingSettings.Values["AUTO_DARK_MODE"] = isAutoDarkMode ? 0 : 1;
            localSettings.Values["AUTO_DARK_START_TIME"] = StartTime.ToString();
            localSettings.Values["AUTO_DARK_END_TIME"] = EndTime.ToString();
            OnShareDataChanged();
        }

        public void UpdateStartTime(TimeSpan t)
        {
            StartTime = t;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["AUTO_DARK_START_TIME"] = StartTime.ToString();
            OnShareDataChanged();
        }

        public void UpdateEndTime(TimeSpan t)
        {
            EndTime = t;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["AUTO_DARK_END_TIME"] = EndTime.ToString();
            OnShareDataChanged();
        }

        public void UpdateUserName(string u)
        {
            UserName = u;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["USER_NAME"] = UserName;
            OnShareDataChanged();
        }

        public void UpdateEmailAdd(string e)
        {
            EmailAdd = e;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["EMAIL_ADDR"] = EmailAdd;
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
            IsCortanaRegistered = true;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["CORTANA"] = 1;
            OnShareDataChanged();
        }

        public void EnableLiveTile()
        {
            IsLiveTileRegistered = true;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["LIVETILE"] = 1;
            OnShareDataChanged();
        }

        public delegate void ShareDataChangedEventHandler();
    }
}
