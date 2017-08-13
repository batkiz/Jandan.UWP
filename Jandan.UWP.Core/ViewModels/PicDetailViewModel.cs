using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;

namespace Jandan.UWP.Core.ViewModels
{
    public class PicDetailViewModel : ViewModelBase
    {
        public BoringPic BoringPicture { get; set; }
        public DuanCommentViewModel _dViewModel { get; set; }

        private bool _isFavourite;
        public bool IsFavourite { get { return _isFavourite; } set { Set(ref _isFavourite, value);  } }

        public PicDetailViewModel()
        {
            _dViewModel = new DuanCommentViewModel();
        }

        public PicDetailViewModel(BoringPic b)
        {
            _dViewModel = new DuanCommentViewModel();
            ReloadPics(b);
        }

        public async void ReloadPics(BoringPic b)
        {
            BoringPicture = b;
            _dViewModel.Update(b.PicID);

            IsFavourite = await CheckIfFavouriteAsync(b);
        }

        private async Task<bool> CheckIfFavouriteAsync(BoringPic b)
        {
            // 读取当前收藏列表
            var boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("boring.xml");
            if (boring_list == null)
            {
                boring_list = new List<BoringPic>();
            }

            var r1= boring_list.Exists(t => t.PicID == b.PicID);

             boring_list = await FileHelper.Current.ReadXmlObjectAsync<List<BoringPic>>("girl.xml");
            if (boring_list == null)
            {
                boring_list = new List<BoringPic>();
            }

            var r2 = boring_list.Exists(t => t.PicID == b.PicID);

            if (r1 || r2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
