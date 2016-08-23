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
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.ViewModels
{
    public class DuanCommentViewModel : ViewModelBase
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


        public DuanCommentViewModel()
        {
            _commentList = new CollectionViewSource();
            _commentList.IsSourceGrouped = true;
        }        

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update(string commentID)
        {
            IsLoadingComments = true;
            //DuanComments?.Clear();
            //HotDuanComments?.Clear();

            CommentList.Source = null;

            var list = await _api.GetDuanComments(commentID);
            if (list != null)
            {
                list?.ForEach((t) =>
                {
                    t.Message = Regex.Replace(t.Message.Replace("<br/>", "\n"), "<.+?>", "");
                });

                // 装入父评论
                list?.ForEach((t) =>
                {
                    if (t.ParentID != "0")
                    {
                        var q = from l in list
                                where l.PostID == t.ParentID
                                select l;
                        if (q.Count() != 0)
                        {
                            t.ParentComment = new ParentDuanComment { AuthorName = q.First().AuthorName, ThreadID = q.First().ThreadID, Message = q.First().Message };
                        }

                        try
                        {
                            t.Message = $"{t.Message}\n回复 {t.ParentComment.AuthorName}:\n{t.ParentComment.Message}";
                        }
                        catch (Exception)
                        {
                            ;
                        }
                    }
                });


                ObservableCollection<DuanCommentInGroup> groups = new ObservableCollection<DuanCommentInGroup>();

                // 将评论根据热否为热评分为两类
                var query = from item in list
                            group item by item.IsHot into newItems
                            orderby newItems.Key descending
                            select new { GroupName = newItems.Key ? "热门评论" : "最新评论", Items = newItems };

                foreach (var g in query)
                {
                    DuanCommentInGroup comments = new DuanCommentInGroup();
                    //comments.Key = g.GroupName;
                    //foreach (var item in g.Items)
                    //{
                    //    comments.Add(item);
                    //}
                    //groups.Add(comments);

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
    }
}
