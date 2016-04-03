using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Models;


namespace Jandan.UWP.ViewModels
{
    public class PicDetailViewModel : ViewModelBase
    {
        public BoringPic BoringPicture { get; set; }

        public PicDetailViewModel(BoringPic b)
        {
            Update(b);
        }

        public async void Update(BoringPic b)
        {
            BoringPicture = b;
        }
        
    }
}
