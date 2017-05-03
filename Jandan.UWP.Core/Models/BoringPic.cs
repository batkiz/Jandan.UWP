using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Jandan.UWP.Core.Models
{
    public enum ImageType { Original, Thumb, Hot, HotThumb};
    /// <summary>
    /// 无聊图
    /// </summary>
    [DataContract]
    public class BoringPic
    {
        [DataMember]
        public string PicID { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public List<ImageItem> Urls { get; set; }
        [DataMember]
        public List<ImageItem> Thumb { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public int VotePositive { get; set; }
        [DataMember]
        public int VoteNegative { get; set; }
        [DataMember]
        public string CommentCount { get; set; }


        public static List<ImageItem> ParseUrl(string JsonString, ImageType it)
        {
            List<ImageItem> url_list = new List<ImageItem>();
            JsonArray jsonArray = JsonArray.Parse(JsonString);

            foreach (var j in jsonArray)
            {
                ImageItem imageUrl;
                switch (it)
                {
                    case ImageType.Original:
                        imageUrl = new ImageItem(j.GetString());
                        break;
                    case ImageType.Thumb:
                        imageUrl = new ImageItem(Regex.Replace(j.GetString(), @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
                        break;
                    case ImageType.Hot:
                        if (Regex.IsMatch(j.GetString(), "(http.+?)\" target"))
                        {
                            var url = Regex.Match(j.GetString(), "(http.+?)\" target");
                            imageUrl = new ImageItem(url.Groups[1].Value);
                        }
                        else
                        {
                            imageUrl = new ImageItem("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                        }
                        break;
                    case ImageType.HotThumb:
                        if (Regex.IsMatch(j.GetString(), "(http.+?)\" target"))
                        {
                            var url = Regex.Match(j.GetString(), "(http.+?)\" target");
                            // 添加低分辨率缩略图
                            imageUrl = new ImageItem(Regex.Replace(url.Groups[1].Value, @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
                        }
                        else
                        {
                            imageUrl = new ImageItem("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                        }
                        break;
                    default:
                        imageUrl = new ImageItem("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                        break;
                }
                
                url_list.Add(imageUrl);
            }

            return url_list;
        }


        public static List<ImageItem> Parse(string JSONString, bool isThumb = false)
        {
            List<ImageItem> url_list = new List<ImageItem>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                ImageItem imageUrl;
                if (isThumb)
                {
                    imageUrl = new ImageItem(Regex.Replace(j.GetString(), @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
                }
                else
                {
                    imageUrl = new ImageItem(j.GetString());
                }
                url_list.Add(imageUrl);
            }

            return url_list;
        }

        //public static List<ImageItem> parseThumb(string JSONString)
        //{
        //    List<ImageItem> url_list = new List<ImageItem>();

        //    JsonArray jsonArray = JsonArray.Parse(JSONString);
        //    foreach (var j in jsonArray)
        //    {
        //        ImageItem imageUrl = new ImageItem(Regex.Replace(j.GetString(), @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
        //        url_list.Add(imageUrl);
        //    }

        //    return url_list;
        //}

        // Hot图的图片Url与无聊图、妹子图不同，需单独处理
        public static List<ImageItem> ParseHot(string JSONString)
        {
            List<ImageItem> url_list = new List<ImageItem>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                // 获取图片Url                
                if (Regex.IsMatch(j.GetString(), "(http.+?)\" target"))
                {
                    var url = Regex.Match(j.GetString(), "(http.+?)\" target");
                    ImageItem imageUrl = new ImageItem(url.Groups[1].Value);
                    url_list.Add(imageUrl);
                }
            }

            return url_list;
        }

        public static List<ImageItem> ParseHotThumb(string JSONString)
        {
            List<ImageItem> url_list = new List<ImageItem>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                // 获取图片Url                
                if (Regex.IsMatch(j.GetString(), "(http.+?)\" target"))
                {
                    var url = Regex.Match(j.GetString(), "(http.+?)\" target");
                    // 添加低分辨率缩略图
                    ImageItem imageUrl = new ImageItem(Regex.Replace(url.Groups[1].Value, @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
                    url_list.Add(imageUrl);
                }                
            }

            return url_list;
        }

        /// <summary>
        /// 从html字符串中直接提取图片网址
        /// </summary>
        /// <param name="JSONString"></param>
        /// <returns></returns>
        public static void ParseURL(string JSONString, out List<ImageItem> scr_list, out List<ImageItem> thumb_list)
        {
            scr_list = new List<ImageItem>();
            thumb_list = new List<ImageItem>();

            if (Regex.IsMatch(JSONString, @"<img src=\""//(.+?)\"""))//"<img src=\\\"//(.+?)\\\""
            {
                var url = Regex.Match(JSONString, @"<img src=\""//(.+?)\""");
                var scr = $"http://{url.Groups[1].Value}";
                ImageItem thumbUrl = new ImageItem(scr);//thumb180
                ImageItem imageUrl = new ImageItem(Regex.Replace(scr, @"(sinaimg\.cn/.+?/)", "sinaimg.cn/large/"));

                scr_list.Add(imageUrl);
                thumb_list.Add(thumbUrl);
            }
        }
    }
}
