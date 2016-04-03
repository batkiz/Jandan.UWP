using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jandan.UWP.Models
{
    /// <summary>
    /// 图片URL
    /// </summary>
    [DataContract]
    public class ImageUrl
    {
        [DataMember]
        public string URL { get; set; }

        public ImageUrl(string url)
        {
            URL = url;
        }
    }
}
