using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jandan.UWP.Models
{
    public class DuanCommentInGroup: List<DuanComment>
    {
        // Group Header
        public string Key { get; set; }

        //// Data Collection
        //public List<DuanComment> ItemContents { get; set; }
    }
}
