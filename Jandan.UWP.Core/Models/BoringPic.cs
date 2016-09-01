using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Jandan.UWP.Core.Models
{
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
        public List<ImageUrl> Urls { get; set; }
        [DataMember]
        public List<ImageUrl> Thumb { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public int VotePositive { get; set; }
        [DataMember]
        public int VoteNegative { get; set; }
        [DataMember]
        public int CommentCount { get; set; }

        public static List<ImageUrl> parse(string JSONString)
        {
            List<ImageUrl> url_list = new List<ImageUrl>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                ImageUrl imageUrl = new ImageUrl(j.GetString());
                url_list.Add(imageUrl);
            }

            return url_list;
        }

        public static List<ImageUrl> parseThumb(string JSONString)
        {
            List<ImageUrl> url_list = new List<ImageUrl>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                ImageUrl imageUrl = new ImageUrl(Regex.Replace(j.GetString(), @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
                url_list.Add(imageUrl);
            }

            return url_list;
        }

        // Hot图的图片Url与无聊图、妹子图不同，需单独处理
        public static List<ImageUrl> parseHot(string JSONString)
        {
            List<ImageUrl> url_list = new List<ImageUrl>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                // 获取图片Url                
                if (Regex.IsMatch(j.GetString(), "(http.+?)\" target"))
                {
                    var url = Regex.Match(j.GetString(), "(http.+?)\" target");
                    ImageUrl imageUrl = new ImageUrl(url.Groups[1].Value);
                    url_list.Add(imageUrl);
                }
            }

            return url_list;
        }

        public static List<ImageUrl> parseHotThumb(string JSONString)
        {
            List<ImageUrl> url_list = new List<ImageUrl>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                // 获取图片Url                
                if (Regex.IsMatch(j.GetString(), "(http.+?)\" target"))
                {
                    var url = Regex.Match(j.GetString(), "(http.+?)\" target");
                    // 添加低分辨率缩略图
                    ImageUrl imageUrl = new ImageUrl(Regex.Replace(url.Groups[1].Value, @"(sinaimg\.cn/.+?/)", "sinaimg.cn/thumb180/"));
                    url_list.Add(imageUrl);
                }                
            }

            return url_list;
        }
    }
}
