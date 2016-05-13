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

            var boring = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("boring_list.json");
            BoringIncrementalLoadingCollection c = new BoringIncrementalLoadingCollection();
            boring?.ForEach((t) =>
            {
                c.Add(t);
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
                c.Add(t);
            });

            Boring = c;

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
    }
}
