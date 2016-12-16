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
using System.Text.RegularExpressions;

namespace Jandan.UWP.Core.ViewModels
{
    public class FavouriteViewModel : ViewModelBase
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

        private ObservableCollection<Fresh> _fresh;
        public ObservableCollection<Fresh> Fresh
        {
            get { return _fresh; }
            set { _fresh = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Duan> _duan;
        public ObservableCollection<Duan> Duan
        {
            get { return _duan; }
            set { _duan = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BoringPic> _pics;
        public ObservableCollection<BoringPic> Pics
        {
            get { return _pics; }
            set { _pics = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BoringPic> _girls;
        public ObservableCollection<BoringPic> Girls
        {
            get { return _girls; }
            set { _girls = value; OnPropertyChanged(); }
        }

        public FavouriteViewModel()
        {
            LoadFavourite();
        }

        private async void LoadFavourite()
        {
            IsLoading = true;
            
            // 从文件中读取收藏的新鲜事
            var fresh_list = await FileHelper.Current.ReadObjectAsync<List<Fresh>>("fav_fresh_list.json");
            ObservableCollection<Fresh> f = new ObservableCollection<Models.Fresh>();
            fresh_list?.ForEach((t) =>
            {
                // todo?

                f.Add(t);
            });
            Fresh = f;

            // 从文件中读取收藏的段子
            var duan_list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("fav_duan_list.json");
            ObservableCollection<Duan> d = new ObservableCollection<Models.Duan>();
            duan_list?.ForEach((t) =>
            {
                // todo?

                d.Add(t);
            });
            Duan = d;

            // 从文件中读取收藏的无聊图
            var pics_list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("fav_pics_list.json");
            ObservableCollection<BoringPic> b = new ObservableCollection<Models.BoringPic>();
            pics_list?.ForEach((t) =>
            {
                // todo?

                b.Add(t);
            });
            Pics = b;

            // 从文件中读取收藏的妹子图
            var girl_list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("fav_girl_list.json");
            ObservableCollection<BoringPic> g = new ObservableCollection<Models.BoringPic>();
            girl_list?.ForEach((t) =>
            {
                // todo?

                g.Add(t);
            });
            Girls = g;

            IsLoading = false;
        }
    }
}
