using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Jandan.UWP.Models
{
    public class Authors
    {
        public string ID { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Nickname { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }

        public static Authors parse(string JSONString)
        {
            Authors author;

            JsonObject jsonObject = JsonObject.Parse(JSONString);

            if (jsonObject == null)
            {
                author = null;
            }
            else
            {
                author = new Authors();
                author.ID           = jsonObject.GetNamedValue("id").ToString();
                author.Slug         = jsonObject.GetNamedString("slug");
                author.Name         = jsonObject.GetNamedString("name");
                author.First_name   = jsonObject.GetNamedString("first_name");
                author.Last_name    = jsonObject.GetNamedString("last_name");
                author.Nickname     = jsonObject.GetNamedString("nickname");
                author.Url          = jsonObject.GetNamedString("url");
                author.Description  = jsonObject.GetNamedString("description");
            }
            return author;
        }

    }
}
