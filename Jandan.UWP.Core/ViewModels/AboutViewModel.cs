using Jandan.UWP.Core.Models;
using System;
using Windows.Storage;

namespace Jandan.UWP.Core.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        //private About _abouts;
        //public About Abouts
        //{
        //    get { return _abouts; }
        //    set { Set(ref _abouts, value); }
        //}

        public AboutViewModel()
        {
            //LoadUpdateInfo();
        }

        //private async void LoadUpdateInfo()
        //{
        //    About a = new About();

        //    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Documents/version-{a.VersionNumber}.txt"));

        //    string fileContent = await FileIO.ReadTextAsync(file);

        //    a.UpdateTextSource = fileContent;

        //    Abouts = a;
        //}
    }
}
