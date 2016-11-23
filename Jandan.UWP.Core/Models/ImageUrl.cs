using System.Runtime.Serialization;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 图片URL
    /// </summary>
    [DataContract]
    public class ImageUrl
    {
        [DataMember]
        public string URL { get; set; } = "ms-appx:///Assets/Square150x150Logo.scale-200.png";

        public ImageUrl(string url)
        {
            URL = url;
        }
    }
}
