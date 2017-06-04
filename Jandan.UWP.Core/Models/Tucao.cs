using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 2017-06-01上线新评论系统
    /// </summary>
    [DataContract]
    public class Tucao
    {
        [DataMember]
        public string comment_ID { get; set; }
        [DataMember]
        public string comment_post_ID { get; set; }
        [DataMember]
        public string comment_author { get; set; }
        [DataMember]
        public string comment_date { get; set; }
        [DataMember]
        public string comment_parent { get; set; }
        [DataMember]
        public string comment_reply_ID { get; set; }
        [DataMember]
        public string vote_positive { get; set; }
        [DataMember]
        public string vote_negative { get; set; }
        [DataMember]
        public int is_tip_user { get; set; }
        [DataMember]
        public int is_jandan_user { get; set; }
    }
}
