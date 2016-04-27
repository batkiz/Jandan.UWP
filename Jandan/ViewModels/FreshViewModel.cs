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

namespace Jandan.UWP.ViewModels
{
    public class FreshViewModel : ViewModelBase
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

        private static double NormalItemWidth = 220;
        private double _itemWidth = 220;
        public double ItemWidth
        {
            get
            {
                return _itemWidth;
            }
            set
            {
                _itemWidth = value;
                OnPropertyChanged();
            }
        }

        private FreshIncrementalLoadingCollection _news;
        public FreshIncrementalLoadingCollection News
        {
            get
            {
                return _news;
            }
            set
            {
                _news = value;
                OnPropertyChanged();
            }
        }

        public FreshViewModel()
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
            var list = await FileHelper.Current.ReadObjectAsync<List<Fresh>>("fresh_list.json");
            FreshIncrementalLoadingCollection c = new FreshIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                c.Add(t);
            });
            News = c;
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            var list = await _api.GetFresh(1);

            FreshIncrementalLoadingCollection c = new FreshIncrementalLoadingCollection();

            list?.ForEach((t) =>
                        {
                            c.Add(t);
                        });

            News = c;

            c.DataLoaded += C_DataLoaded;
            c.DataLoading += C_DataLoading;

            IsLoading = false;
        }

        internal double UpdateItemWidth(double pageWidth)
        {
            int count = (int)(pageWidth / NormalItemWidth);
            return ItemWidth = pageWidth / count;
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
