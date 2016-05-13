using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using System.Runtime.Serialization;

namespace Jandan.UWP.Models
{
    [DataContract]
    public class About
    {
        [DataMember]
        public string VersionNumber { get; } = "0.2.0";
        [DataMember]
        public string AuthorName { get; } = "Ray Litchi";
        [DataMember]
        public string HelpAndSuggestion { get; } = "如果您有任何好的意见或者建议，欢迎发邮件至raysworld@qq.com反馈\n\n找不到妹子图的童鞋可以多点标题栏的“无聊图”三个字几次试试";
        [DataMember]
        public string DenoteText { get; } = "如果觉得好用，请赏赐一碗泡面钱，谢谢支持\n支付婊:\nraysworld@qq.com, *睿";
        [DataMember]
        public string UpdateTextSource { get; set; } = "";
        
    }
}
