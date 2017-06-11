namespace Jandan.UWP.Core.HTTP
{
    static class ServiceURL
    {
        public static string URL_FRESH_NEWS = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_recent_posts&include=url,date,tags,author,title,comment_count,custom_fields&page={0}&custom_fields=thumb_c,views&dev=1";

        public static string URL_FRESH_NEWS_DETAIL = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_post&include=content,tags&id={0}";

        // 评论列表
        public static string URL_FRESH_COMMENTS = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_post&include=comments&id={0}";

        // 对新鲜事发表评论
        public static string URL_PUSH_COMMENT = "http://i.jandan.net/?oxwlxojflwblxbsapi=respond.submit_comment";

        // 用于获取评论数量
        // 段子等号后加comment-[comment_ID]
        public static string URL_COMMENT_COUNTS = "http://jandan.duoshuo.com/api/threads/counts.json?threads=";

        // 评论列表
        // 段子等号后加comment-[comment_ID]
        //public static string URL_COMMENT_LIST = "http://jandan.duoshuo.com/api/threads/listPosts.json?thread_key=";
        public static string URL_COMMENT_LIST = "http://i.jandan.net/tucao/";


        // 发表段子、无聊图评论
        //public static string URL_PUSH_DUAN_COMMENT = "http://jandan.duoshuo.com/api/posts/create.json";
        public static string URL_PUSH_DUAN_COMMENT = "http://i.jandan.net/jandan-tucao.php";

        // 投票
        public static string URL_VOTE = "http://i.jandan.net/index.php?acv_ajax=true&option={0}";

        // 段子
        public static string URL_DUANZI = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_duan_comments&page={0}";

        // 无聊图
        public static string URL_BORING_PICTURE = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_pic_comments&page={0}";

        // 妹子图
        public static string URL_MEIZI = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_ooxx_comments&page={0}";

        // 小电影
        public static string URL_VIDEOS = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_video_comments&page=";

        // 热门无聊图
        public static string URL_HOTPICS = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_pic&dev=1&include=";
        
        // 热门段子
        public static string URL_HOTDUAN = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_duan&dev=1&include=";
        
        // 优评
        public static string URL_HOTCOMM = "http://i.jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_comments&include=";

        // 多说接口
        public static string URL_DUOSHUO_WEIBO  = @"https://jandan.duoshuo.com/login/weibo/?sso=1&redirect_uri=http://jandan.net/";
        public static string URL_DUOSHUO_QQ     = @"https://jandan.duoshuo.com/login/qq/?sso=1&redirect_uri=http://jandan.net/";
        public static string URL_DUOSHUO_BAIDU  = @"https://jandan.duoshuo.com/login/baidu/?sso=1&redirect_uri=http://jandan.net/";
        public static string URL_DUOSHUO_DOUBAN = @"https://jandan.duoshuo.com/login/douban/?sso=1&redirect_uri=http://jandan.net/";
        public static string URL_DUOSHUO_RENREN = @"https://jandan.duoshuo.com/login/renren/?sso=1&redirect_uri=http://jandan.net/";
        public static string URL_DUOSHUO_KAIXIN = @"https://jandan.duoshuo.com/login/kaixin/?sso=1&redirect_uri=http://jandan.net/";

        // 不受欢迎公式(oo + xx) >= 50 && (oo / xx) < 0.618
        // 包含NSFW


    }
}
