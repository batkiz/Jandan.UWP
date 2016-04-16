using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Data;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;
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

        public DuanViewModel()
        {
            Update();

            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
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

                c.Add(t);
            });

            Duans = c;

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
