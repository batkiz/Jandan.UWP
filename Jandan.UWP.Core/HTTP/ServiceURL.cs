namespace Jandan.UWP.Core.HTTP
{
    static class ServiceURL
    {
        #region 获取内容类API
        // 获取新鲜事列表
        public static string API_GET_FRESH_NEWS = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_recent_posts&include=url,date,tags,author,title,excerpt,comment_count,comment_status,custom_fields&page={0}&custom_fields=thumb_c,views&dev=1";
        // 获取段子列表
        public static string API_GET_DUANZI = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_duan_comments&page={0}";
        // 获取无聊图列表
        public static string API_GET_BORING_PICTURE = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_pic_comments&page={0}";
        // 获取妹子图列表
        public static string API_GET_MEIZI = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_ooxx_comments&page={0}";


        // 获取热门无聊图列表
        public static string API_GET_HOTPICS = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_pic&dev=1&include=";
        // 获取热门段子列表
        public static string API_GET_HOTDUAN = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_duan&dev=1&include=";
        // 获取优评列表
        public static string API_GET_HOTCOMM = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_comments&include=";


        // 根据文章ID获取新鲜事内容
        public static string API_GET_FRESH_NEWS_DETAIL = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_post&include=content,tags&id={0}";
        #endregion


        #region 获取评论类API
        // 获取段子或无聊图评论
        public static string API_POST_COMMENT_LIST = "http://i.jandan.net/tucao/";

        // 获取新鲜事评论
        public static string API_GET_FRESH_COMMENTS = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_post&include=comments&id={0}";
        #endregion


        #region 发表评论类API
        // 发表新鲜事评论
        public static string URL_PUSH_COMMENT = "http://i.jandan.net/?oxwlxojflwblxbsapi=respond.submit_comment";

        // 发表段子或无聊图评论
        public static string URL_PUSH_DUAN_COMMENT = "http://i.jandan.net/jandan-tucao.php";

        // 顶或踩
        public static string API_POST_VOTE = "http://i.jandan.net/index.php?acv_ajax=true&option={0}";
        #endregion


        #region 投稿类API
        //TODO

        #endregion
    }
}
