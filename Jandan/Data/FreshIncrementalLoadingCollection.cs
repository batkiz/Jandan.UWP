﻿using System;
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
    public class FreshIncrementalLoadingCollection : ObservableCollection<Fresh>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _buzy = false;
        private bool _has_more_items = false;
        private string _last_fresh_id;

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

        public FreshIncrementalLoadingCollection(string last_fresh_id)
        {
            _last_fresh_id = last_fresh_id;
            HasMoreItems = true;
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _buzy = true;
            var actualCount = 0;
            List<Fresh> list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
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
                    Add(t);
                });
                _last_fresh_id = list.Last().ID;
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
            }
            if (DataLoaded != null)
            {
                DataLoaded();
            }
            _buzy = false;

            return new LoadMoreItemsResult { Count = (uint)actualCount };
        }
    }
}
