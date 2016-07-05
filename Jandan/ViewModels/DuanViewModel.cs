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
    public class DuanViewModel : ViewModelBase
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

        private DuanIncrementalLoadingCollection _duans;
        public DuanIncrementalLoadingCollection Duans
        {
            get
            {
                return _duans;
            }
            set
            {
                _duans = value;
                OnPropertyChanged();
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
                Update();
                DataShareManager.Current.UpdateUnwelcome(_is_show_unwelcome);
            }
        }

        public DuanViewModel()
        {
            //Update();
            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
            LoadCache();
            Update();
        }

        public async void LoadCache()
        {
            IsLoading = true;

            var list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("duan_list.json");
            DuanIncrementalLoadingCollection c = new DuanIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                if (IsItemAdded(t))
                {
                    c.Add(t);
                }
            });
            Duans = c;

            IsLoading = false;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            var list = await _api.GetDuan(1);

            DuanIncrementalLoadingCollection c = new DuanIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                var msg = t.Content;                
                t.Content = Regex.Replace(msg, "<.+?>", "");

                if (IsItemAdded(t))
                {
                    c.Add(t); 
                }
            });

            Duans = c;

            c.DataLoaded += C_DataLoaded;
            c.DataLoading += C_DataLoading;

            IsLoading = false;
        }

        private bool IsItemAdded(Duan t)
        {
            bool isPassedUnWel = true;
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
            return (isPassedUnWel);
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

        public async Task<string> Vote(string ID, bool isLike)
        {
            return await _api.Vote(ID, isLike);
        }
    }
}
