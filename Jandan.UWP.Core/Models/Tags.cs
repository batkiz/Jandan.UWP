using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Jandan.UWP.Models
{
    public class Tags
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public static List<Tags> parse(string JSONString)
        {
            List<Tags> tags = new List<Tags>();

            JsonArray jsonArray = JsonArray.Parse(JSONString);
            foreach (var j in jsonArray)
            {
                tags.Add(new Tags {Id=(int)(j.GetObject()).GetNamedNumber("id"),
                                    Description=(j.GetObject()).GetNamedString("description"),
                                    Title=(j.GetObject()).GetNamedString("title")
                });
            }
            
            return tags;
        }

    }
}