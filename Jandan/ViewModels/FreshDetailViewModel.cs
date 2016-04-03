using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jandan.UWP.Data;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;


namespace Jandan.UWP.ViewModels
{
    public class FreshDetailViewModel : ViewModelBase
    {
        private APIService _api = new APIService();

        public bool isLoading { get; set; }

        private FreshDetail _freshDetails;
        public FreshDetail FreshDetails { get { return _freshDetails; } set { _freshDetails = value; OnPropertyChanged(); } }

        public FreshDetailViewModel(Fresh fresh)
        {
            _freshDetails = new FreshDetail() { FreshInfo = fresh, FreshContentSlim = "", FreshContentEx = "" };

            Update(_freshDetails.FreshInfo);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update(Fresh fresh)
        {
            var list = await _api.GetFreshDetail(fresh);

            if (list == null)
            {
                isLoading = true;
            }
            else
            {
                // 根据设备类型设置不同的CSS文件
                //string css = SetCss();
                // 处理文档中各种格式标记
                string text_content = SetContent(list.FreshContentSlim);
                // 设置文章中的标题
                string title = SetTitle(fresh);

                string header_slim = "<head>" + SetCss(false) + "</head>";
                string header_ex = "<head>" + SetCss(true) + "</head>";
                string body = "<body>" + title + text_content + "</body>";

                list.FreshContentEx = "<!DOCTYPE HTML>\n<html>" + header_ex + body + "</html>";
                list.FreshContentSlim = "<!DOCTYPE HTML>\n<html>" + header_slim + body + "</html>";

                isLoading = false;
            }
            FreshDetails = list;
        }

        private static string SetTitle(Fresh fresh)
        {
            return @"<em>" + fresh.Author.Name + " @ " + fresh.Tag[0].Title + @"</em>"
                         + @"<h2 class=""FreshTitle"">" + fresh.Title + "</h2>"
                         + "<em>" + fresh.Date + @"</em>";
        }

        private string SetContent(string raw)
        {
            // 删去<img>标签外的<p>标签
            string s = Regex.Replace(raw, "<p>(<img src.+?/>)</p>", "$1") + "</body>";

            // 删去落款[]部分<a>标签外的<p>标签
            // todo

            // 调整视频大小
            s = Regex.Replace(s, @"<p>(<iframe.+?></iframe>.+?</a>])</p>", "$1");
            //s = Regex.Replace(s, @"(<iframe height=)\d{1,5}( width=)\d{1,5}(.+?></iframe>)", @"${1}100%${2}100%${3}");
            s = Regex.Replace(s, @"<p>(<embed.+?/>)</p>", "$1");
            //s = Regex.Replace(s, @"(<embed.+?width="")\d{ 1,5}("".+?height="")\d{ 1,5}(""/>)", @"${1}100%${2}100%${3}");
            return s;

        }

        /// <summary>
        /// 根据设备类型设置不同的CSS文件
        /// </summary>
        /// <returns></returns>
        private string SetCss(bool isExtended)
        {
            string ext_sym = isExtended ? "_ex" : "";

            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

            string css = @"<LINK href=""ms-appx-web:///Style/{0}.css"" rel = ""stylesheet"" type = ""text/css"" media = ""screen"" /> ";
            if (string.Equals(platformFamily, "Windows.Mobile"))
            {
                css = string.Format(css, "jandan_mobile" + ext_sym);
            }
            else // Windows.Desktop
            {
                css = string.Format(css, "jandan_desktop" + ext_sym);
            }

            if (DataShareManager.Current.IsDarkMode)
            {
                css += @"<style>body{background:black; color:gray}</style>";
            }            

            return css;
        }
    }
}
