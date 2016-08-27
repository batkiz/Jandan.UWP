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
