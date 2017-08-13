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
                case "seeFreshNews":
                    await seeFreshNews();
                    break;
            }
            _taskDerral.Complete();
        }

        private async Task seeFreshNews()
        {
            var msgback = new VoiceCommandUserMessage();

            var news = await GetNews();
            var p = news.Take(10).ToList();

            // 取10条最新新鲜事            
            var picTiles = new List<VoiceCommandContentTile>();
            int i = 1;
            foreach (var item in p)
            {
                var file_name = Path.GetFileName(item.Thumb_c);
                var uri = new Uri(item.Thumb_c, UriKind.Absolute);

                var newsTile = new VoiceCommandContentTile();

                newsTile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                newsTile.Image = await StorageFile.CreateStreamedFileFromUriAsync(
                    file_name,
                    uri,
                    RandomAccessStreamReference.CreateFromUri(uri));
                newsTile.AppContext = item;
                newsTile.Title = $"{item.Title}";
                newsTile.TextLine1 = $"@{item.Tag[0].Title}";
                newsTile.TextLine2 = $"by {item.Author.Name}";
                newsTile.TextLine3 = "";

                picTiles.Add(newsTile);
                i++;
            }

            msgback.DisplayMessage = msgback.SpokenMessage = "找到最近的十条新鲜事";
            var response = VoiceCommandResponse.CreateResponse(msgback, picTiles);

            await _serviceConnection.ReportSuccessAsync(response);
        }

        private async Task<List<Fresh>> GetNews()
        {
            APIService _api = new APIService();

            try
            {
                var response = await APIService.GetFresh(1);
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

        private void _serviceConnection_VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this._taskDerral != null)

            {

                this._taskDerral.Complete();

            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
#endif

            if (this._taskDerral != null)

            {

                //Complete the service deferral

                this._taskDerral.Complete();

            }
        }

        // Number随机数个数
        // minNum随机数下限
        // maxNum随机数上限
        public int[] GetRandomArray(int Number, int minNum, int maxNum)
        {
            int j;
            int[] b = new int[Number];
            Random r = new Random();
            for (j = 0; j < Number; j++)
            {
                int i = r.Next(minNum, maxNum + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (b[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    b[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return b;
        }

        private async Task seeABoringPic()
        {
            var msgback = new VoiceCommandUserMessage();

            var pics = await GetPics();
            int[] pic_idx = GetRandomArray(3, 0, pics.Count);

            // 挑三张最新无聊图
            var p = new List<BoringPic>
            {
                pics[pic_idx[0]], 
                pics[pic_idx[1]],
                pics[pic_idx[2]]
            };
            var picTiles = new List<VoiceCommandContentTile>();
            int i = 1;
            foreach (var item in p)
            {
                var file_name = Path.GetFileName(item.Thumb[0].URL);
                var uri = new Uri(item.Thumb[0].URL, UriKind.Absolute);

                var picTile = new VoiceCommandContentTile();
                
                picTile.ContentTileType = VoiceCommandContentTileType.TitleWith280x140IconAndText;
                picTile.Image = await StorageFile.CreateStreamedFileFromUriAsync(
                    file_name, 
                    uri, 
                    RandomAccessStreamReference.CreateFromUri(uri));
                picTile.AppContext = item;
                picTile.Title = $"第{i}张：";
                picTile.TextLine1 = item.Content == null ? "" : item.Content;
                picTile.TextLine2 = $"来自{item.Author}上传的无聊图";
                picTile.TextLine3 = "";

                picTiles.Add(picTile);
                i++;
            }
            
            msgback.DisplayMessage = msgback.SpokenMessage = "找到最近的三张无聊图";
            var response = VoiceCommandResponse.CreateResponse(msgback, picTiles);

            await _serviceConnection.ReportSuccessAsync(response);
        }

        private async Task<List<BoringPic>> GetPics()
        {
            APIService _api = new APIService();

            try
            {
                var response = await APIService.GetBoringPics(1);
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

            var jokeStr = $"{d.Content}";

            if (jokeStr.Length >= 300)
            {
                jokeTile.TextLine1 = jokeStr.Substring(0, 100);
                jokeTile.TextLine2 = jokeStr.Substring(100, 100);
                jokeTile.TextLine3 = jokeStr.Substring(200, 100);
            }
            else if (jokeStr.Length >= 200)
            {
                jokeTile.TextLine1 = jokeStr.Substring(0, 100);
                jokeTile.TextLine2 = jokeStr.Substring(100, 100);
                jokeTile.TextLine3 = jokeStr.Substring(200);
            }
            else if (jokeStr.Length >= 100)
            {
                jokeTile.TextLine1 = jokeStr.Substring(0, 100);
                jokeTile.TextLine2 = jokeStr.Substring(100);
            }
            else
            {
                jokeTile.TextLine1 = jokeStr;
            }

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
                var response = await APIService.GetDuan(1);
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
