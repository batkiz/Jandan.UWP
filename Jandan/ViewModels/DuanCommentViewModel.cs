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
    public class DuanCommentViewModel : ViewModelBase
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

        private ObservableCollection<DuanComment> _duanComments;
        public ObservableCollection<DuanComment> DuanComments
        {
            get { return _duanComments; }
            set { _duanComments = value; OnPropertyChanged(); }
        }                

        public DuanCommentViewModel()
        {
            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update(string commentID)
        {
            IsLoading = true;
            DuanComments?.Clear();

            var list = await _api.GetDuanComments(commentID);
            
            ObservableCollection<DuanComment> c = new ObservableCollection<Models.DuanComment>();
            
            list?.ForEach((t) =>
            {
                string msg = t.Message;
                t.Message = Regex.Replace(msg.Replace("<br/>", "\n"), "<.+?>", "");

                c.Add(t);
            });

            DuanComments = c;
            IsLoading = false;
        }
    }
}
