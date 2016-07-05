using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Data;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;
using Jandan.UWP.Tools;
using System.Text.RegularExpressions;

namespace Jandan.UWP.ViewModels
{
    public class BoringViewModel : ViewModelBase
    {
        private APIService _api = new APIService();

        private bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                _is_loading = value;
                OnPropertyChanged();
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
                _is_show_nsfw = value;
                OnPropertyChanged();
                UpdateBoringPics();
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
                _is_show_unwelcome = value;
                OnPropertyChanged();
                UpdateBoringPics();
                DataShareManager.Current.UpdateUnwelcome(_is_show_unwelcome);
            }
        }

        private BoringIncrementalLoadingCollection _boring;
        public BoringIncrementalLoadingCollection Boring
        {
            get { return _boring; }
            set { _boring = value; OnPropertyChanged(); }
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
                //bool isPassedNSFW = true, isPassedUnWel = true;
                //if (!IsShowNSFW && t.Content.Contains("NSFW"))
                //{
                //    isPassedNSFW = false;
                //}
                //if (!IsShowUnwelcome)
                //{
                //    int oo = t.VotePositive;
                //    int xx = t.VoteNegative;
                //    if ((oo + xx) >= 50 && ((double)oo / (double)xx) < 0.618)
                //    {
                //        t.Content += "\n\nUnwelcome";
                //        isPassedUnWel = false;
                //    }
                //}
                //if (isPassedNSFW && isPassedUnWel)
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
            var list = await _api.GetBoringPics(1);

            BoringIncrementalLoadingCollection c = new BoringIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                var comment = t.Content.Replace("\n", "").Replace("\r", "");
                t.Content = comment;

                //bool isPassedNSFW = true, isPassedUnWel = true;
                //if (!IsShowNSFW && t.Content.Contains("NSFW"))
                //{
                //    isPassedNSFW = false;
                //}
                //if (!IsShowUnwelcome)
                //{
                //    int oo = t.VotePositive;
                //    int xx = t.VoteNegative;
                //    if ((oo + xx) >= 50 && ((double)oo / (double)xx) < 0.618)
                //    {
                //        t.Content += "\n\nUnwelcome";
                //        isPassedUnWel = false;
                //    }
                //}
                //if (isPassedNSFW && isPassedUnWel)
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
    }
}
