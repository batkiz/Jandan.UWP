using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
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
        public string Date { get; set; }
        [DataMember]
        public int VotePositive { get; set; }
        [DataMember]
        public int VoteNegative { get; set; }
        [DataMember]
        public string CommentCount { get; set; }
        [DataMember]
        public string ContentType { get; set; }
    }
}
