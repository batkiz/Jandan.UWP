using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace Jandan.UWP.LiveTileTask
{
    public sealed class JandanLiveTileTask : IBackgroundTask
    {
        private const string TileTemplateXml = @"
        <tile>
            <visual>
                <binding template='TileSmall' hint-textStacking='center'>
                    <text hint-align='center'>煎蛋</text>
                </binding>
        
                <binding template='TileMedium' branding='name' displayName='煎蛋UWP'>
                    <image src='{3}' placement='peek' hint-crop='circle'/>
                    <text hint-wrap='true'>{0}</text>
                    <text hint-style='captionSubtle'>@{2}</text>
                    <text hint-wrap='true' hint-style='captionSubtle'>{1}</text>
                </binding>
        
                <binding template='TileWide' branding='nameAndLogo' displayName='煎蛋UWP'>
                    <image src='{3}' placement='background'/>
                    <text hint-wrap='true'>{0}</text>
                    <text hint-style='captionSubtle'>@{2}</text>
                    <text hint-style='captionSubtle'>{1}</text>
                </binding>
        
                <binding template='TileLarge' branding='nameAndLogo' displayName='煎蛋UWP'>
                    <image src='{3}'/>
                    <group>
                        <subgroup>
                            <text hint-wrap='true'>{0}</text>
                            <text hint-style='captionSubtle'>@{2}</text>
                            <text hint-style='captionSubtle'>{1}</text>
                        </subgroup>
                    </group>
                </binding>
            </visual>
        </tile>";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            await GetLatestFreshNews();

            deferral.Complete();
        }

        private IAsyncOperation<string> GetLatestFreshNews()
        {
            try
            {
                return AsyncInfo.Run(token => GetNews());
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        private async Task<string> GetNews()
        {
            APIService _api = new APIService();

            try
            {
                var response = await APIService.GetFresh(1);
                if (response?.Count != 0)
                {
                    List<Fresh> n = new List<Fresh>();
                    response.ForEach((t) =>
                    {
                        if (!t.Title.Contains("NSFW") & !t.Title.Contains("OOXX"))
                        {
                            n.Add(t);
                        }
                    });

                    var news = n.Take(n.Count > 5 ? 5 : n.Count).ToList();
                    UpdatePrimaryTile(news);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        private void UpdatePrimaryTile(List<Fresh> news)
        {
            if (news == null || !news.Any())
            {
                return;
            }

            try
            {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueueForWide310x150(true);
                updater.EnableNotificationQueueForSquare150x150(true);
                updater.EnableNotificationQueueForSquare310x310(true);
                updater.EnableNotificationQueue(true);
                updater.Clear();

                foreach (var n in news)
                {
                    var doc = new XmlDocument();
                    var xml = string.Format(
                        TileTemplateXml, 
                        n.Title, 
                        n.Date, 
                        n.Tag[0].Title, 
                        n.Thumb_c);
                    doc.LoadXml(WebUtility.HtmlDecode(xml), new XmlLoadSettings
                    {
                        ProhibitDtd = false,
                        ValidateOnParse = false,
                        ElementContentWhiteSpace = false,
                        ResolveExternals = false
                    });

                    updater.Update(new TileNotification(doc));
                }
            }
            catch (Exception)
            {
                // ignored
            }

        }
    }
}
