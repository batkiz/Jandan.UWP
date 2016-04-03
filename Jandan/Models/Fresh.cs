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
    /// 新鲜事
    /// </summary>
    [DataContract]
    public class Fresh
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Date{ get; set; }
        [DataMember]
        public string Thumb_c { get; set; }
        [DataMember]
        public string Comment_count { get; set; }
        [DataMember]
        public Authors Author { get; set; }
        [DataMember]
        public List<Tags> Tag { get; set; }
    }
}
