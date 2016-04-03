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
    /// 段子
    /// </summary>
    [DataContract]
    public class Duan
    {
        [DataMember]
        public string DuanID { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public int VotePositive { get; set; }
        [DataMember]
        public int VoteNegative { get; set; }
        [DataMember]
        public int CommentCount { get; set; }
    }
}
