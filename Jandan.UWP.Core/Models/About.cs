﻿using System.Runtime.Serialization;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Jandan.UWP.Core.Models
{
    [DataContract]
    public class About
    {
        [DataMember]
        public string VersionNumber { get; } = "2.3.3";
        [DataMember]
        public string AuthorName { get; } = "Ray Litchi（感谢同是蛋友的Tim童鞋强烈扫除各种bug）";
        [DataMember]
        public string HelpAndSuggestion { get; } = "如果您有任何好的意见或者建议，欢迎发邮件至raysworld@qq.com反馈\n\n找不到妹子图的童鞋可以多点标题栏的“无聊图”三个字几次试试";
        [DataMember]
        public string DenoteText { get; } = "如果觉得好用，请赏赐一个五星评价，谢谢支持 :-)支付宝raysworld@qq.com(*睿)";
        [DataMember]
        public string UpdateTextSource { get; set; } = "";
        [DataMember]
        public string ModelType { get; set; } = SystemInformation.DeviceModel;

    }
}
