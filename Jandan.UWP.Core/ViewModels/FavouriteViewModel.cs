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
using Windows.Data.Xml.Dom;
using Windows.Storage;
using System.Runtime.Serialization;
using Windows.Storage.Streams;
using System.IO;

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

        public void Update()
        {
            LoadFavourite();
        }

        private async void LoadFavourite()
        {
            IsLoading = true;

            // 从文件中读取收藏的新鲜事
            //var u = new Uri("ms-appx:///Favourite/fav_fresh.xml");
            //var file = await StorageFile.GetFileFromApplicationUriAsync(u);
            //var fresh_list = await XmlDocument.LoadFromFileAsync(file);

            //var nodes = fresh_list.SelectSingleNode("ItemLists");
            //int node_count = 0;
            //foreach (var n in nodes.ChildNodes)
            //{
            //    if (n.NodeType == NodeType.ElementNode)
            //    {
            //        node_count++;
            //    }
            //}

            var fresh_list = await FileHelper.Current.ReadXmlObjectAsync<List<Fresh>>("fresh.xml");
            ObservableCollection<Fresh> f = new ObservableCollection<Fresh>();
            fresh_list?.ForEach((t) =>
            {
                // todo?

                f.Add(t);
            });
            Fresh = f;

            var duan_list = await FileHelper.Current.ReadXmlObjectAsync<List<Duan>>("duan.xml");
            ObservableCollection<Duan> d = new ObservableCollection<Duan>();
            duan_list?.ForEach((t) =>
            {
                // todo?

                d.Add(t);
            });
            Duan = d;

            var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("boring.xml");
            ObservableCollection<BoringPic> b = new ObservableCollection<BoringPic>();
            boring_list?.ForEach((t) =>
            {
                // todo?

                b.Add(t);
            });
            Pics = b;

            var girl_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("girl.xml");
            ObservableCollection<BoringPic> g = new ObservableCollection<BoringPic>();
            girl_list?.ForEach((t) =>
            {
                // todo?

                g.Add(t);
            });
            Girls = g;
            //var fresh_list = await FileHelper.Current.ReadObjectAsync<List<Fresh>>("ms-appx:///Favourite/fav_fresh.xml");
            //ObservableCollection<Fresh> f = new ObservableCollection<Models.Fresh>();
            //fresh_list?.ForEach((t) =>
            //{
            //    // todo?

            //    f.Add(t);
            //});
            //Fresh = f;

            //// 从文件中读取收藏的段子
            //var duan_list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("fav_duan_list.json");
            //ObservableCollection<Duan> d = new ObservableCollection<Models.Duan>();
            //duan_list?.ForEach((t) =>
            //{
            //    // todo?

            //    d.Add(t);
            //});
            //Duan = d;

            //// 从文件中读取收藏的无聊图
            //var pics_list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("fav_pics_list.json");
            //ObservableCollection<BoringPic> b = new ObservableCollection<Models.BoringPic>();
            //pics_list?.ForEach((t) =>
            //{
            //    // todo?

            //    b.Add(t);
            //});
            //Pics = b;

            //// 从文件中读取收藏的妹子图
            //var girl_list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("fav_girl_list.json");
            //ObservableCollection<BoringPic> g = new ObservableCollection<Models.BoringPic>();
            //girl_list?.ForEach((t) =>
            //{
            //    // todo?

            //    g.Add(t);
            //});
            //Girls = g;

            IsLoading = false;
        }
    }
}
