using System.Runtime.Serialization;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;

namespace Jandan.UWP.Core.Models
{
    /// <summary>
    /// 图片URL
    /// </summary>
    [DataContract]
    public class ImageItem
    {
        [DataMember]
        public string URL { get; set; } = "ms-appx:///Assets/Square150x150Logo.scale-200.png";

        public ImageItem(string url)
        {
            URL = url;
        }        
    }
}
