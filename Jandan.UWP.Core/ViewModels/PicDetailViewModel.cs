using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Core.Models;


namespace Jandan.UWP.Core.ViewModels
{
    public class PicDetailViewModel : ViewModelBase
    {
        public BoringPic BoringPicture { get; set; }

        public PicDetailViewModel(BoringPic b)
        {
            Update(b);
        }

        public void Update(BoringPic b)
        {
            BoringPicture = b;
        }
        
    }
}
