using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.Core.ViewModels
{
    public class FreshCommentViewModel :ViewModelBase
    {
        private APIService _api = new APIService();

        private bool _is_loading_comments;
        public bool IsLoadingComments
        {
            get
            {
                return _is_loading_comments;
            }
            set
            {
                _is_loading_comments = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource _commentList;
        public CollectionViewSource CommentList
        {
            get { return _commentList; }
            set { _commentList = value; OnPropertyChanged(); }
        }

        private string _textbox_comment;
        public string TextBoxComment
        {
            get
            {
                return _textbox_comment;
            }
            set
            {
                _textbox_comment = value;
                OnPropertyChanged();
            }
        }

        public string ThreadId { get; set; }
        public string ParentId { get; set; }

        public FreshCommentViewModel()
        {
            _commentList = new CollectionViewSource();
            _commentList.IsSourceGrouped = true;

            ThreadId = "";
            ParentId = "";
            TextBoxComment = "";
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
            IsLoadingComments = true;

            CommentList.Source = null;

            var list = await _api.GetFreshComments(commentID);

            if (list != null)
            {
                list?.ForEach((t) =>
                {
                    var m = t.Message;
                    m = Regex.Replace(m, "<br />", "");
                    m = Regex.Replace(m, "<p>", "");
                    m = Regex.Replace(m, "</p>", "");
                    m = Regex.Replace(m, "@<a href=.+?\">(.+?)</a>(.+?)", "@ <Bold>${1}</Bold>\n${2}");
                    t.Message = m;
                });

                ObservableCollection<FreshCommentInGroup> groups = new ObservableCollection<FreshCommentInGroup>();
                // 将评论根据热否为热评分为两类
                var query = from item in list
                            group item by item.IsHot into newItems
                            orderby newItems.Key descending
                            select new { GroupName = newItems.Key ? "热门评论" : "最新评论", Items = newItems };
                foreach (var g in query)
                {
                    FreshCommentInGroup comments = new FreshCommentInGroup();            

                    comments.Key = g.GroupName;
                    if (comments.Key.Contains("热门"))
                    {
                        foreach (var item in g.Items)
                        {
                            comments.Add(item);
                        }
                    }
                    else // 最新
                    {
                        foreach (var item in list)
                        {
                            comments.Add(item);
                        }
                    }
                    groups.Add(comments);
                }

                CommentList.Source = groups;
            }
            IsLoadingComments = false;
        }

        public async Task<string> PostComment(string comment)
        {
            var msg = await _api.PostFreshComment(comment);

            return msg;
        }
    }
}
