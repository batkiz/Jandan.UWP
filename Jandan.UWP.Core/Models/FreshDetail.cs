using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 新鲜事详情页
    /// </summary>
    [DataContract]
    public class FreshDetail
    {
        [DataMember]
        public Fresh FreshInfo { get; set; }
        [DataMember]
        public string FreshContentSlim { get; set; }
        [DataMember]
        public string FreshContentEx { get; set; }
    }
}
