using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jandan.UWP.Data;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;

namespace Jandan.UWP.ViewModels
{
    public class FreshCommentViewModel :ViewModelBase
    {
        private APIService _api = new APIService();

        private ObservableCollection<FreshComment> _freshComments;
        public ObservableCollection<FreshComment> FreshComments
        {
            get { return _freshComments; }
            set { _freshComments = value; OnPropertyChanged(); }
        }

        public FreshCommentViewModel()
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
            FreshComments?.Clear();

            var list = await _api.GetFreshComments(commentID);

            ObservableCollection<FreshComment> c = new ObservableCollection<FreshComment>();

            list?.ForEach((t) =>
            {
                var m = t.Message;
                m = Regex.Replace(m, "<br />", "");
                m = Regex.Replace(m, "<p>", "");
                m = Regex.Replace(m, "</p>", "");
                m = Regex.Replace(m, "@<a href=.+?\">(.+?)</a>(.+?)", "@ <Bold>${1}</Bold>\n${2}");
                t.Message = m;

                c.Add(t);
            });

            FreshComments = c;
        }
    }
}
