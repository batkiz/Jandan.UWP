using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;
using Jandan.UWP.ViewModels;
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.Core.ViewModels
{
    // 重写用户评论View Model, 成熟后替换原DuanCommentViewModel
    public class UserCommentViewModel : ViewModelBase
    {
        private APIService _api = new APIService();

        // 载入评论标志
        private bool _is_loading_comments;
        public bool IsLoadingComments
        {
            get { return _is_loading_comments; }
            set { _is_loading_comments = value; OnPropertyChanged(); }
        }

        // 评论数据
        private CollectionViewSource _user_comments;
        public CollectionViewSource UserComments
        {
            get { return _user_comments; }
            set { _user_comments = value; OnPropertyChanged(); }
        }

        // 待回复的父评论（值为string.empty表示发表新评论，不回复）
        private string _parent_id;
        public string ParentId
        {
            get { return _parent_id; }
            set { _parent_id = value; OnPropertyChanged(); }
        }

        // 待回复的post编号（post指无聊图、段子或妹子图）
        private string _post_id;
        public string PostId
        {
            get { return _post_id; }
            set { _post_id = value; OnPropertyChanged(); }
        }

        // 待回复的评论内容
        private string _response_content;
        public string ResponseComment
        {
            get { return _response_content; }
            set { _response_content = value; OnPropertyChanged(); }
        }
    }
}
