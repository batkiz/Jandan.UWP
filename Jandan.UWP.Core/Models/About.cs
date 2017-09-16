using System.Runtime.Serialization;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel;
using Windows.Storage;
using System;

namespace Jandan.UWP.Core.Models
{
    [DataContract]
    public sealed class About
    {
        [DataMember]
        public string VersionNumber { get; }
        [DataMember]
        public string AuthorName { get; }
        [DataMember]
        public string HelpAndSuggestion { get; }
        [DataMember]
        public string DenoteText { get; }
        [DataMember]
        public string UpdateTextSource { get; set; }
        [DataMember]
        public string ModelType { get; set; }

        private static About _current;
        public static About Current
        {
            get
            {
                if (_current == null) _current = new About();
                return _current;
            }
        }

        public About()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            VersionNumber = $"{version.Major}.{version.Minor}.{version.Build}";
            AuthorName = "Ray Litchi（感谢同是蛋友的Tim童鞋强烈扫除各种bug，感谢zhuimeng.me的特别帮助）";
            HelpAndSuggestion = "如果您有任何好的意见或者建议，欢迎发邮件至raysworld@qq.com反馈\n\n找不到妹子图的童鞋可以多点标题栏的“无聊图”三个字几次试试";
            DenoteText = "如果觉得好用，请赏赐一个五星评价，谢谢支持 :-)支付宝raysworld@qq.com(*睿)";
            LoadUpdateInfo();
            ModelType = SystemInformation.DeviceModel;
        }

        private async void LoadUpdateInfo()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Documents/version.txt"));

            UpdateTextSource = await FileIO.ReadTextAsync(file);
        }
    }
}
