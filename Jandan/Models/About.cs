﻿using System;
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
        public string VersionNumber { get; } = "0.1.0";
        [DataMember]
        public string AuthorName { get; } = "Ray Litchi";
        [DataMember]
        public string HelpAndSuggestion { get; } = "如果您有任何好的意见或者建议，欢迎发邮件至ray8_3@aliyun.com反馈";
        [DataMember]
        public string DenoteText { get; } = "如果觉得好用，请赏赐一碗泡面钱，谢谢支持\n支付婊:\nraysworld@qq.com, *睿";
        [DataMember]
        public string UpdateTextSource { get; set; } = "";

        //public About()
        //{
        //    LoadUpdateInfo();
        //}

        //public async void LoadUpdateInfo()
        //{
        //    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(string.Format("ms-appx:///Documents/version-{0}.txt", VersionNumber)));

        //    string fileContent = await FileIO.ReadTextAsync(file);

        //    UpdateTextSource = fileContent;
        //}

    }
}
