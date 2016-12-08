using System.Collections.Generic;

namespace Jandan.UWP.Core.Models
{
    public class CommentInGroup<T> : List<T>
    {
        // Group Header
        public string Key { get; set; }
    }
}
