using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 新鲜事评论
    /// </summary>
    [DataContract]
    public class BestFreshComment
    {
        [DataMember]
        public string PostID { get; set; }
        [DataMember]
        public string CommentID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string AuthorName { get; set; }
        [DataMember]
        public string PostDate { get; set; }
        [DataMember]
        public int Like { get; set; }
        [DataMember]
        public int Dislike { get; set; }
        [DataMember]
        public FreshDetail FreshNews { get; set; }
    }
}
