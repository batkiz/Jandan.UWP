using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Jandan.UWP.Models;

namespace Jandan.UWP.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private About _abouts;
        public About Abouts
        {
            get { return _abouts; }
            set { _abouts = value; OnPropertyChanged(); }
        }

        public AboutViewModel()
        {
            LoadUpdateInfo();
        }

        private async void LoadUpdateInfo()
        {
            About a = new About();

            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Documents/version-{a.VersionNumber}.txt"));

            string fileContent = await FileIO.ReadTextAsync(file);

            a.UpdateTextSource = fileContent;

            Abouts = a;
        }
    }
}
