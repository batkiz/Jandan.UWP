using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;

namespace Jandan.UWP.CortanaTask
{
    public sealed class JandanCortanaTask : IBackgroundTask
    {
        BackgroundTaskDeferral _taskDerral;
        VoiceCommandServiceConnection _serviceConnection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _taskDerral = taskInstance.GetDeferral();
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            taskInstance.Canceled += TaskInstance_Canceled;


            // 验证是否调用了正确的app service
            if (details == null || details.Name != "JandanCortanaService")
            {
                _taskDerral.Complete();
                return;
            }
            _serviceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(details);

            _serviceConnection.VoiceCommandCompleted += _serviceConnection_VoiceCommandCompleted;

            // 获取被识别的语音命令
            var cmd = await _serviceConnection.GetVoiceCommandAsync();
            switch (cmd.CommandName)
            {
                case "tellAJoke":
                    await TellAJoke();
                    break;
                case "seeABoringPic":
                    await seeABoringPic();
                    break;
            }
            _taskDerral.Complete();
        }

        private void _serviceConnection_VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this._taskDerral != null)

            {

                this._taskDerral.Complete();

            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");

            if (this._taskDerral != null)

            {

                //Complete the service deferral

                this._taskDerral.Complete();

            }
        }

        private async Task seeABoringPic()
        {
            var msgback = new VoiceCommandUserMessage();

            var pics = await GetPics();
            // 挑三张最新无聊图
            var p = pics.Take(1).ToList();
            var picTiles = new List<VoiceCommandContentTile>();
            int i = 1;
            foreach (var item in p)
            {
                var file_name = Path.GetFileName(item.Thumb[0].URL);
                var uri = new Uri(item.Thumb[0].URL, UriKind.Absolute);
                //var file_name = Path.GetFileName("http://www.baidu.com/img/bd_logo1.png");
                //var uri = new Uri("http://www.baidu.com/img/bd_logo1.png");

                var picTile = new VoiceCommandContentTile();
                
                picTile.ContentTileType = VoiceCommandContentTileType.TitleWith280x140IconAndText;
                picTile.Image = await StorageFile.CreateStreamedFileFromUriAsync(
                    file_name, 
                    uri, 
                    RandomAccessStreamReference.CreateFromUri(uri));
                picTile.AppContext = item;
                picTile.Title = $"第{i}张：来自{item.Author}上传的无聊图";
                picTile.TextLine1 = item.Content == null ? "" : item.Content;
                picTile.TextLine2 = file_name;
                picTile.TextLine3 = "";

                picTiles.Add(picTile);
                i++;
            }
            
            msgback.DisplayMessage = msgback.SpokenMessage = "找到最新的三张无聊图";
            var response = VoiceCommandResponse.CreateResponse(msgback, picTiles);

            await _serviceConnection.ReportSuccessAsync(response);
        }

        private async Task<List<BoringPic>> GetPics()
        {
            APIService _api = new APIService();

            try
            {
                var response = await _api.GetBoringPics(1);
                if (response?.Count != 0)
                {
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
        }

        private async Task TellAJoke()
        {
            var msgback = new VoiceCommandUserMessage();

            var jokes = await GetDuans();
            // 随机挑一个段子
            var d = jokes[new Random(DateTime.Now.Millisecond).Next(jokes.Count)];
            string msg = $"找到一枚段子：";

            var jokeTile = new VoiceCommandContentTile();
            jokeTile.ContentTileType = VoiceCommandContentTileType.TitleWithText;
            jokeTile.Title = $"来自{d.Author}的段子：";
            jokeTile.TextLine1 = $"{d.Content}";
            var jokeTiles = new List<VoiceCommandContentTile>();
            jokeTiles.Add(jokeTile);

            msgback.DisplayMessage = msgback.SpokenMessage = msg;
            var response = VoiceCommandResponse.CreateResponse(msgback, jokeTiles);

            await _serviceConnection.ReportSuccessAsync(response);
        }

        private async Task<List<Duan>> GetDuans()
        {
            APIService _api = new APIService();

            try
            {
                var response = await _api.GetDuan(1);
                if (response?.Count != 0)
                {
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
        }
    }
}
