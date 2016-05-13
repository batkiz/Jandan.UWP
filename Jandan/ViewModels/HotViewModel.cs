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
    public class HotViewModel : ViewModelBase
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

        private ObservableCollection<BoringPic> _pics;
        public ObservableCollection<BoringPic> Pics
        {
            get { return _pics; }
            set { _pics = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Duan> _duan;
        public ObservableCollection<Duan> Duan
        {
            get { return _duan; }
            set { _duan = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BestFreshComment> _comm;
        public ObservableCollection<BestFreshComment> Comm
        {
            get { return _comm; }
            set { _comm = value; OnPropertyChanged(); }
        }

        public HotViewModel()
        {
            LoadCache();
            UpdateHotPics();
            UpdateHotDuan();
            UpdateHotComm();

            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        public async void LoadCache()
        {
            IsLoading = true;
            var pics = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("hot_pics_list.json");
            ObservableCollection<BoringPic> d = new ObservableCollection<BoringPic>();
            pics?.ForEach((t) =>
            {
                d.Add(t);
            });
            Pics = d;
            IsLoading = false;

            IsLoading = true;
            var duan = await FileHelper.Current.ReadObjectAsync<List<Duan>>("hot_duan_list.json");
            ObservableCollection<Duan> e = new ObservableCollection<Duan>();
            duan?.ForEach((t) =>
            {
                e.Add(t);
            });
            Duan = e;
            IsLoading = false;

            IsLoading = true;
            var best = await FileHelper.Current.ReadObjectAsync<List<BestFreshComment>>("BestFreshComment.json");
            ObservableCollection<BestFreshComment> f = new ObservableCollection<BestFreshComment>();
            best?.ForEach((t) =>
            {
                f.Add(t);
            });
            Comm = f;
            IsLoading = false;
        }

        public async Task<string> Vote(string ID, bool isLike)
        {
            return await _api.Vote(ID, isLike);
        }

        public async void UpdateHotPics()
        {
            IsLoading = true;
            var list = await _api.GetHotPics();

            ObservableCollection<BoringPic> c = new ObservableCollection<BoringPic>();
            list?.ForEach((t) =>
            {
                var comment = t.Content.Replace("\n", "").Replace("\r", "").Replace("[查看原图]", "");
                comment = Regex.Replace(comment, "OO.+?XX.+?]", "");
                t.Content = comment;
                c.Add(t);
            });

            Pics = c;

            IsLoading = false;
        }

        public async void UpdateHotDuan()
        {
            IsLoading = true;
            var list = await _api.GetHotDuan();

            ObservableCollection<Duan> c = new ObservableCollection<Duan>();
            list?.ForEach((t) =>
            {
                var comment = t.Content;
                comment = Regex.Replace(comment, "OO.+?XX.+?]", "");
                t.Content = comment;
                c.Add(t);
            });
            Duan = c;

            IsLoading = false;
        }

        public async void UpdateHotComm()
        {
            IsLoading = true;
            var list = await _api.GetHotComments();

            ObservableCollection<BestFreshComment> c = new ObservableCollection<BestFreshComment>();
            list?.ForEach((t) =>
            {
                var title = t.Title;
                title = $"@{title}";
                t.Title = title;

                c.Add(t);
            });

            Comm = c;

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
    }
}
