using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace Jandan.UWP.Core.HTTP
{
    /// <summary>
    /// api服务基类
    /// </summary>
    public class APIBaseService
    {
        // 打印调试信息到调试输出窗口
        private static void Printlog(string info)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + info);
#endif
        }

        /// <summary>
        /// 获取指定URL的Json数据
        /// </summary>
        /// <param name="url">获取数据的URL</param>
        /// <returns>获取到的Json对象</returns>
        protected static async Task<JsonObject> GetJson(string url)
        {
            using (var request = new HttpHelperRequest(new Uri(url), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    try
                    {
                        string json = await response.GetTextResultAsync();
                        if (json != null)
                        {
                            Printlog("请求Json数据成功 URL：" + url);
                            return JsonObject.Parse(json);
                        }
                        else
                        {
                            Printlog("请求Json数据为空 URL：" + url);
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        Printlog("请求Json数据失败 URL：" + url);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定URL的Html数据
        /// </summary>
        /// <param name="url">获取数据的URL</param>
        /// <returns>获取到的字符串，表示html页面</returns>
        protected static async Task<string> GetHtml(string url)
        {
            using (var request = new HttpHelperRequest(new Uri(url), HttpMethod.Get))
            {
                using (var response = await HttpHelper.Instance.SendRequestAsync(request))
                {
                    try
                    {
                        string html = await response.GetTextResultAsync();
                        if (html != null)
                        {
                            Printlog("请求html数据成功 URL：" + url);
                            return html;
                        }
                        else
                        {
                            Printlog("请求html数据为空 URL：" + url);
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        Printlog("请求html数据失败 URL：" + url);
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定URL的图像数据
        /// </summary>
        /// <param name="url">获取数据的URL</param>
        /// <returns>获取到的图像</returns>
        protected static async Task<BitmapImage> GetImage(string url)
        {
            // Load a specific image from the cache. If the image is not in the cache, ImageCache will try to download and store it
            var bitmapImage = await ImageCache.Instance.GetFromCacheAsync(new Uri(url));

            return bitmapImage;
        }
    }
}
