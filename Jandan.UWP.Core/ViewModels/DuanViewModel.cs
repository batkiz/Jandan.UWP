using Jandan.UWP.Core.Data;
using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jandan.UWP.Core.ViewModels
{
    public class DuanViewModel : ViewModelBase, IDisposable
    {
        private APIService _api = new APIService();

        private bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                _is_loading = value;
                OnPropertyChanged();
            }
        }

        private DuanIncrementalLoadingCollection _duans;
        public DuanIncrementalLoadingCollection Duans
        {
            get
            {
                return _duans;
            }
            set
            {
                _duans = value;
                OnPropertyChanged();
            }
        }

        private bool _is_show_unwelcome;
        public bool IsShowUnwelcome
        {
            get
            {
                return _is_show_unwelcome;
            }
            set
            {
                _is_show_unwelcome = value;
                OnPropertyChanged();
                Update();
                DataShareManager.Current.UpdateUnwelcome(_is_show_unwelcome);
            }
        }

        public DuanViewModel()
        {
            //Update();
            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
            LoadCache();
            Update();
        }

        public async void LoadCache()
        {
            IsLoading = true;

            var list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("duan_list.json");
            DuanIncrementalLoadingCollection c = new DuanIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                if (IsItemAdded(t))
                {
                    c.Add(t);
                }
            });
            Duans = c;

            IsLoading = false;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void Update()
        {
            IsLoading = true;
            var list = await _api.GetDuan(1);

            DuanIncrementalLoadingCollection c = new DuanIncrementalLoadingCollection();
            list?.ForEach((t) =>
            {
                var msg = t.Content;                
                t.Content = Regex.Replace(msg, "<.+?>", "");//去除文字中的html标记
                //t.Content = $"<!DOCTYPE html><html><head><title>{t.DuanID}</title></head><body><p>{t.Content}</p></body></html>";

                if (IsItemAdded(t))
                {
                    c.Add(t); 
                }
            });

            Duans = c;

            c.DataLoaded += C_DataLoaded;
            c.DataLoading += C_DataLoading;

            IsLoading = false;
        }

        private bool IsItemAdded(Duan t)
        {
            bool isPassedUnWel = true;
            if (!IsShowUnwelcome)
            {
                int oo = t.VotePositive;
                int xx = t.VoteNegative;
                if ((oo + xx) >= 50 && ((double)oo / (double)xx) < 0.618)
                {
                    t.Content += "\n\nUnwelcome";
                    isPassedUnWel = false;
                }
            }
            return (isPassedUnWel);
        }

        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoading()
        {
            IsLoading = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoaded()
        {
            IsLoading = false;
        }

        public async Task<string> Vote(string ID, bool isLike)
        {
            return await _api.Vote(ID, isLike);
        }

        public async Task<string> Vote(Duan duan, bool isLike)
        {
            var d = Duans;

            var msg = await _api.Vote(duan.DuanID, isLike);

            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }
            else if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    d[Duans.IndexOf(duan)].VotePositive++;
                }
                else
                {
                    d[Duans.IndexOf(duan)].VoteNegative++;
                }
            }

            Duans = d;

            return msg;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DuanViewModel() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
