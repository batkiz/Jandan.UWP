using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Jandan.UWP.Models
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
    }
}
