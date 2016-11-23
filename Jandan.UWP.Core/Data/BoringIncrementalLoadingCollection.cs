using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Jandan.UWP.Core.Data
{
    public class BoringIncrementalLoadingCollection : ObservableCollection<BoringPic>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _buzy = false;
        private bool _has_more_items = false;

        public bool HasMoreItems
        {
            get
            {
                if (_buzy) { return false; }
                else { return _has_more_items; }
            }
            private set
            {
                _has_more_items = value;
            }
        }

        public event DataLoadedEventHandler DataLoaded;
        public event DataLoadingEventHandler DataLoading;

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        public BoringIncrementalLoadingCollection()
        {
            HasMoreItems = true;
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _buzy = true;
            var actualCount = 0;
            List<BoringPic> list = null;
            try
            {
                DataLoading?.Invoke();
                list = await _api.GetBoringPics(DataShareManager.Current.BoringItemPage++);
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
                list.ForEach((t) =>
                {
                    //if (DataShareManager.Current.ReadedList.Contains(t.ID))
                    //{
                    //    t.Readed = true;
                    //}
                    if (DataShareManager.Current.isNoImageMode)
                    {
                        t.Urls = t.Thumb;
                    }

                    var comment = t.Content.Replace("\n", "").Replace("\r", "");
                    t.Content = comment;

                    bool isPassedNSFW = true, isPassedUnWel = true;
                    if (!DataShareManager.Current.IsShowNSFW && t.Content.Contains("NSFW"))
                    {
                        isPassedNSFW = false;
                    }
                    if (!DataShareManager.Current.IsShowUnwelcome)
                    {
                        int oo = t.VotePositive;
                        int xx = t.VoteNegative;
                        if ((oo + xx) >= 50 && ((double)oo / (double)xx) < 0.618)
                        {
                            t.Content += "\n\nUnwelcome";
                            isPassedUnWel = false;
                        }
                    }
                    if (isPassedNSFW && isPassedUnWel)
                    {
                        Add(t);
                    }                    
                });
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
                --DataShareManager.Current.BoringItemPage;
            }
            DataLoaded?.Invoke();
            _buzy = false;

            return new LoadMoreItemsResult { Count = (uint)actualCount };
        }
    }
}
