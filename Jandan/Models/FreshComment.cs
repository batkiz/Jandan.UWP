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
        public string AuthorAvatar { get; set; }
        [DataMember]
        public string PostDate { get; set; }
    }
}
