using Jandan.UWP.Core.Data;
using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using Microsoft.Toolkit.Uwp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jandan.UWP.Core.ViewModels
{
    public class BoringViewModel : ViewModelBase
    {
        private bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                Set(ref _is_loading, value);
            }
        }

        private bool _is_show_nsfw;
        public bool IsShowNSFW
        {
            get
            {
                return _is_show_nsfw;
            }
            set
            {
                Set(ref _is_show_nsfw, value);
                //UpdateBoringPics();
                DataShareManager.Current.UpdateNSFW(_is_show_nsfw);
            }
        }

        private bool _is_show_unwelcome;
        public bool IsShowUnwelcome
        {
            get
            {
                return _is_show_unwelcome;
            }
            set
            {
                Set(ref _is_show_unwelcome, value);
                //UpdateBoringPics();
                DataShareManager.Current.UpdateUnwelcome(_is_show_unwelcome);
            }
        }

        private BoringIncrementalLoadingCollection _boring;
        public BoringIncrementalLoadingCollection Boring
        {
            get { return _boring; }
            set { Set(ref _boring, value);  }
        }

        public BoringViewModel()
        {
            LoadCache();
            UpdateBoringPics();

            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        public async void LoadCache()
        {
            IsLoading = true;

            IsShowNSFW = DataShareManager.Current.IsShowNSFW;
            IsShowUnwelcome = DataShareManager.Current.IsShowUnwelcome;

            var boring = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("boring_list.json");
            BoringIncrementalLoadingCollection c = new BoringIncrementalLoadingCollection();
            boring?.ForEach((t) =>
            {
                if (DataShareManager.Current.IsNoImageMode && ConnectivityHelper.isMeteredConnection)
                {
                    t.Urls = t.Thumb;
                }

                if (IsItemAdded(t))
                {
                    c.Add(t);
                }                
            });
            Boring = c;

            IsLoading = false;
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void UpdateBoringPics()
        {
            IsLoading = true;
            var list = await APIService.GetBoringPics(1);

            BoringIncrementalLoadingCollection c = new BoringIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                var comment = t.Content.Replace("\n", "").Replace("\r", "");
                t.Content = comment;

                if (DataShareManager.Current.IsNoImageMode && ConnectivityHelper.isMeteredConnection)
                {
                    t.Urls = t.Thumb;
                }
                
                if (IsItemAdded(t))
                {
                    c.Add(t);
                }
            });

            Boring = c;

            c.DataLoaded += C_DataLoaded;
            c.DataLoading += C_DataLoading;

            IsLoading = false;
        }

        private bool IsItemAdded(BoringPic t)
        {
            bool isPassedNSFW = true, isPassedUnWel = true;
            if (!IsShowNSFW && t.Content.Contains("NSFW"))
            {
                isPassedNSFW = false;
            }
            if (!IsShowUnwelcome)
            {
                int oo = t.VotePositive;
                int xx = t.VoteNegative;
                if ((oo + xx) >= 50 && ((double)oo / (double)xx) < 0.618)
                {
                    t.Content += "\n\nUnwelcome";
                    isPassedUnWel = false;
                }
            }
            return (isPassedNSFW && isPassedUnWel);
        }

        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoading()
        {
            IsLoading = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoaded()
        {
            IsLoading = false;
        }

        public async Task<string> Vote(BoringPic boring, bool isLike)
        {
            var b = Boring;

            var msg = await APIService.Vote(boring.PicID, isLike);

            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }
            else if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    b[Boring.IndexOf(boring)].VotePositive++;
                }
                else
                {
                    b[Boring.IndexOf(boring)].VoteNegative++;
                }
            }

            Boring = b;

            return msg;
        }
    }
}
