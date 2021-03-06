﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Core.Data;
using Jandan.UWP.Core.HTTP;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using System.Text.RegularExpressions;

namespace Jandan.UWP.Core.ViewModels
{
    public class HotViewModel : ViewModelBase
    {
        private bool _is_loading;
        public bool IsLoading
        {
            get            {                return _is_loading;            }
            set            {                Set(ref _is_loading, value);            }
        }

        private ObservableCollection<BoringPic> _pics;
        public ObservableCollection<BoringPic> Pics
        {
            get { return _pics; }
            set { Set(ref _pics, value); }
        }

        private ObservableCollection<Duan> _duan;
        public ObservableCollection<Duan> Duan
        {
            get { return _duan; }
            set { Set(ref _duan, value); }
        }

        private ObservableCollection<BestFreshComment> _comm;
        public ObservableCollection<BestFreshComment> Comm
        {
            get { return _comm; }
            set { Set(ref _comm, value); }
        }

        public HotViewModel()
        {
            //LoadCache();
            UpdateHotPics();
            UpdateHotDuan();
            UpdateHotComm();

            //DataShareManager.Current.ShareDataChanged += Current_ShareDataChanged;
        }

        //private void Current_ShareDataChanged()
        //{
        //    Stories.ToList().ForEach((s) => s.Readed = s.Readed);
        //}

        public async void LoadCache()
        {
            IsLoading = true;
            var pics = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("hot_pics_list.json");
            ObservableCollection<BoringPic> d = new ObservableCollection<BoringPic>();
            pics?.ForEach((t) =>
            {
                if (DataShareManager.Current.IsNoImageMode && ConnectivityHelper.isMeteredConnection)
                {
                    t.Urls = t.Thumb;
                }

                d.Add(t);
            });
            Pics = d;
            IsLoading = false;

            IsLoading = true;
            var duan = await FileHelper.Current.ReadObjectAsync<List<Duan>>("hot_duan_list.json");
            ObservableCollection<Duan> e = new ObservableCollection<Duan>();
            duan?.ForEach((t) =>
            {
                //t.Content = $"<!DOCTYPE html><html><head><title>{t.DuanID}</title></head><body><p>{t.Content}</p></body></html>";
                t.Content.Replace("&#", "\\&#");

                e.Add(t);
            });
            Duan = e;
            IsLoading = false;

            IsLoading = true;
            var best = await FileHelper.Current.ReadObjectAsync<List<BestFreshComment>>("BestFreshComment.json");
            ObservableCollection<BestFreshComment> f = new ObservableCollection<BestFreshComment>();
            best?.ForEach((t) =>
            {
                f.Add(t);
            });
            Comm = f;
            IsLoading = false;
        }

        public async Task<string> Vote(Duan duan, bool isLike)
        {
            var d = Duan;

            var msg = await APIService.Vote(duan.DuanID, isLike);

            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }
            else if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    d[Duan.IndexOf(duan)].VotePositive++;
                }
                else
                {
                    d[Duan.IndexOf(duan)].VoteNegative++;
                }
            }

            Duan = d;

            return msg;
        }

        public async Task<string> Vote(BoringPic boring, bool isLike)
        {
            var b = Pics;

            var msg = await APIService.Vote(boring.PicID, isLike);

            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }
            else if (msg.Contains("THANK YOU"))
            {
                if (isLike)
                {
                    b[Pics.IndexOf(boring)].VotePositive++;
                }
                else
                {
                    b[Pics.IndexOf(boring)].VoteNegative++;
                }
            }

            Pics = b;

            return msg;
        }

        public async void UpdateHotPics()
        {
            IsLoading = true;
            var list = await APIService.GetHotPics();

            ObservableCollection<BoringPic> c = new ObservableCollection<BoringPic>();
            list?.ForEach((t) =>
            {
                if (DataShareManager.Current.IsNoImageMode && ConnectivityHelper.isMeteredConnection)
                {
                    t.Urls = t.Thumb;
                }

                var comment = t.Content.Replace("\n", "").Replace("\r", "").Replace("[查看原图]", "");
                comment = Regex.Replace(comment, "OO.+?XX.+?]", "");
                t.Content = comment;
                c.Add(t);
            });

            Pics = c;

            IsLoading = false;
        }

        public async void UpdateHotDuan()
        {
            IsLoading = true;
            var list = await APIService.GetHotDuan();

            ObservableCollection<Duan> c = new ObservableCollection<Duan>();
            list?.ForEach((t) =>
            {
                var comment = t.Content;
                comment = Regex.Replace(comment, "OO.+?XX.+?]", "");
                t.Content = comment;

                t.Content = t.Content.Replace(@"&#8230;", "……");
                c.Add(t);
            });
            Duan = c;

            IsLoading = false;
        }

        public async void UpdateHotComm()
        {
            IsLoading = true;
            var list = await APIService.GetHotComments();

            ObservableCollection<BestFreshComment> c = new ObservableCollection<BestFreshComment>();
            list?.ForEach((t) =>
            {
                var title = t.Title;
                title = $"@{title}";
                t.Title = title;

                c.Add(t);
            });

            Comm = c;

            IsLoading = false;
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
    }
}
