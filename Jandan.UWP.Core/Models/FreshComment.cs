using System;
using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 新鲜事评论
    /// </summary>
    [DataContract]
    public class FreshComment
    {
        [DataMember]
        public string PostID { get; set; }
        [DataMember]
        public string ThreadID { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string ParentID { get; set; }
        [DataMember]
        public string AuthorName { get; set; }
        [DataMember]
        public Uri AuthorAvatarUri { get; set; }
        [DataMember]
        public string PostDate { get; set; }
    }
}
