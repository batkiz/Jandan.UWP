using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 段子评论
    /// </summary>
    [DataContract]
    public class Tucao
    {
        [DataMember]
        public string PostID { get; set; }
        [DataMember]
        public string ThreadID { get; set; }
        [DataMember]
        public string ThreadKey { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public List<ImageItem> Urls { get; set; }
        [DataMember]
        public string ContentType { get; set; }
        [DataMember]
        public string ParentID { get; set; }
        [DataMember]
        public ParentDuanComment ParentComment { get; set; }
        [DataMember]
        public string AuthorName { get; set; }
        [DataMember]
        public Uri AuthorAvatarUri { get; set; }
        [DataMember]
        public string PostDate { get; set; }
        [DataMember]
        public int Like { get; set; }
        [DataMember]
        public int Dislike { get; set; }
        [DataMember]
        public bool IsHot { get; set; } = false;
        [DataMember]
        public int VipUser { get; set; } = 0; //00-普通游客 10 IsJandanUser-小编蓝名 01 IsTipUser-打赏橙名
        
        [DataMember]
        public string OrderNumber { get; set; }
    }

    [DataContract]
    public class ParentDuanComment
    {
        [DataMember]
        public string ThreadID { get; set; }
        [DataMember]
        public string AuthorName { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public ParentDuanComment Parent { get; set; }
    }
}
