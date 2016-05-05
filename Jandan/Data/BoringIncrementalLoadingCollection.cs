using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Jandan.UWP.HTTP;
using Jandan.UWP.Models;
using Jandan.UWP.ViewModels;

namespace Jandan.UWP.Data
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
                    Add(t);
                });
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
            }
            DataLoaded?.Invoke();
            _buzy = false;

            return new LoadMoreItemsResult { Count = (uint)actualCount };
        }
    }
}
