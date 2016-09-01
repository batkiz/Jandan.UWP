using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Core.Data;
using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;

namespace Jandan.UWP.Core.ViewModels
{
    public class MeiziViewModel : ViewModelBase
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

        private MeiziIncrementalLoadingCollection _meizi;
        public MeiziIncrementalLoadingCollection Meizi
        {
            get
            {
                return _meizi;
            }
            set
            {
                _meizi = value;
                OnPropertyChanged();
            }
        }

        public MeiziViewModel()
        {
            LoadCache();
            Update();

            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        public async void LoadCache()
        {
            var list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("girl_list.json");
            MeiziIncrementalLoadingCollection c = new MeiziIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                c.Add(t);
            });
            Meizi = c;
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            var list = await _api.GetMeiziPics(1);

            MeiziIncrementalLoadingCollection c = new MeiziIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                c.Add(t);
            });

            Meizi = c;

            c.DataLoaded += C_DataLoaded;
            c.DataLoading += C_DataLoading;

            IsLoading = false;
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
            var b = Meizi;

            var msg = await _api.Vote(boring.PicID, isLike);

            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }
            else if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    b[Meizi.IndexOf(boring)].VotePositive++;
                }
                else
                {
                    b[Meizi.IndexOf(boring)].VoteNegative++;
                }
            }

            Meizi = b;

            return msg;
        }
    }
}
