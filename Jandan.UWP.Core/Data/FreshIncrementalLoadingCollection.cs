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
    public class FreshIncrementalLoadingCollection : ObservableCollection<Fresh>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _buzy = false;
        private bool _has_more_items = false;

        public bool HasMoreItems
        {
            get
            {
                if (_buzy)
                {
                    return false;
                }
                else
                {
                    return _has_more_items;
                }
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

        public FreshIncrementalLoadingCollection()
        {
            HasMoreItems = true;
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _buzy = true;
            var actualCount = 0;
            List<Fresh> list = null;
            try
            {
                DataLoading?.Invoke();
                list = await _api.GetFresh(DataShareManager.Current.FreshNewsPage++);
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
                        t.Thumb_c = "ms-appx:///Assets/No_Image_150.png";
                    }
                    Add(t);
                });
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
                --DataShareManager.Current.FreshNewsPage;
            }
            DataLoaded?.Invoke();
            _buzy = false;

            return new LoadMoreItemsResult { Count = (uint)actualCount };
        }
    }
}
