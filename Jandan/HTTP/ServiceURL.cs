using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jandan.UWP.HTTP
{
    static class ServiceURL
    {
        public static string URL_FRESH_NEWS = "http://jandan.net/?oxwlxojflwblxbsapi=get_recent_posts&include=url,date,tags,author,title,comment_count,custom_fields&custom_fields=thumb_c,views&dev=1&page={0}";

        public static string URL_FRESH_NEWS_DETAIL = "http://i.jandan.net/?oxwlxojflwblxbsapi=get_post&include=content,tags&id={0}";

        // 评论列表
        public static string URL_FRESH_COMMENTS = "http://jandan.net/?oxwlxojflwblxbsapi=get_post&include=comments&id={0}";

        // 对新鲜事发表评论
        public static string URL_PUSH_COMMENT = "http://jandan.net/?oxwlxojflwblxbsapi=respond.submit_comment";

        // 用于获取评论数量
        // 段子等号后加comment-[comment_ID]
        public static string URL_COMMENT_COUNTS = "http://jandan.duoshuo.com/api/threads/counts.json?threads=";

        // 评论列表
        // 段子等号后加comment-[comment_ID]
        public static string URL_COMMENT_LIST = "http://jandan.duoshuo.com/api/threads/listPosts.json?thread_key=";

        //// 发表评论
        //public static string URL_PUSH_COMMENT = "http://jandan.duoshuo.com/api/posts/create.json";

        // 投票
        public static string URL_VOTE = "http://i.jandan.net/index.php?acv_ajax=true&option={0}";

        // 段子
        public static string URL_DUANZI = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_duan_comments&page={0}";

        // 无聊图
        public static string URL_BORING_PICTURE = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_pic_comments&page={0}";

        // 妹子图
        public static string URL_MEIZI = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_ooxx_comments&page={0}";

        // 小电影
        public static string URL_VIDEOS = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_video_comments&page=";

        // 热门无聊图
        public static string URL_HOTPICS = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_pic&dev=1&include=";
        
        // 热门段子
        public static string URL_HOTDUAN = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_duan&dev=1&include=";
        
        // 优评
        public static string URL_HOTCOMM = "http://jandan.net/?oxwlxojflwblxbsapi=jandan.get_hottest_comments&include=";
        
        // 不受欢迎公式(oo + xx) >= 50 && (oo / xx) < 0.618
        // 包含NSFW
    }
}
