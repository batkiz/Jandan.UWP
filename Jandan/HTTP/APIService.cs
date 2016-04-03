using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;
using Jandan.UWP.Models;
using Jandan.UWP.Tools;
using Jandan.UWP.ViewModels;

namespace Jandan.UWP.HTTP
{
    /// <summary>
    /// api服务类  将接收到json字符串格式化成实体类
    /// </summary>
    public class APIService : APIBaseService
    {
        private string _local_path = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;

        /// <summary>
        /// 新鲜事列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Fresh>> GetFresh(int pageNum)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<Fresh> list = await FileHelper.Current.ReadObjectAsync<List<Fresh>>("fresh_list.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_FRESH_NEWS, pageNum));

                    if (json != null)
                    {
                        List<Fresh> list = new List<Fresh>();
                        var posts = json["posts"];
                        if (posts != null)
                        {
                            JsonArray ja = posts.GetArray();
                            foreach (var j in ja)
                            {
                                list.Add(new Fresh
                                {
                                    ID = (j.GetObject())["id"].GetNumber().ToString(),
                                    Title = (j.GetObject())["title"].GetString(),
                                    Author = Authors.parse((j.GetObject())["author"].GetObject().ToString()),
                                    Tag = Tags.parse((j.GetObject())["tags"].ToString()),
                                    Comment_count = (j.GetObject())["comment_count"].GetNumber().ToString(),
                                    Thumb_c = ((j.GetObject())["custom_fields"].GetObject())["thumb_c"].GetArray().GetStringAt(0),
                                    Date = (j.GetObject())["date"].GetString(),
                                    Url = (j.GetObject())["url"].GetString()
                                });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<Fresh>>(list, "fresh_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 新鲜事详情
        /// </summary>
        /// <returns></returns>
        public async Task<FreshDetail> GetFreshDetail(Fresh fresh)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    FreshDetail list = await FileHelper.Current.ReadObjectAsync<FreshDetail>(string.Format("freshDetail-{0}.json", fresh.ID));
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_FRESH_NEWS_DETAIL, fresh.ID));

                    if (json != null)
                    {

                        var post = json["post"];
                        string htmlContent = (post.GetObject())["content"].GetString();

                        FreshDetail list = new FreshDetail() { FreshInfo = fresh, FreshContentSlim = htmlContent, FreshContentEx = htmlContent };
                        await FileHelper.Current.WriteObjectAsync<FreshDetail>(list, string.Format("freshDetail-{0}.json", fresh.ID));
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 段子列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Duan>> GetDuan(int pageNum)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<Duan> list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("duan_list.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_DUANZI, pageNum));

                    if (json != null)
                    {
                        List<Duan> list = new List<Duan>();
                        var posts = json["comments"];
                        if (posts != null)
                        {
                            JsonArray ja = posts.GetArray();
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentID);

                                list.Add(new Duan
                                {
                                    DuanID = (j.GetObject())["comment_ID"].GetString(),
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["comment_content"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = (int)jsonCommentCount["response"].GetObject().GetNamedObject(CommentID).GetNamedNumber("comments")
                                });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<Duan>>(list, "duan_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 无聊图列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BoringPic>> GetBoringPics(int pageNum)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<BoringPic> list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("boring_list.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_BORING_PICTURE, pageNum));

                    if (json != null)
                    {
                        List<BoringPic> list = new List<BoringPic>();
                        var posts = json["comments"];
                        if (posts != null)
                        {
                            JsonArray ja = posts.GetArray();
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentID);

                                list.Add(new BoringPic
                                {
                                    PicID = (j.GetObject())["comment_ID"].GetString(),
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["text_content"].GetString(),
                                    Urls = BoringPic.parse((j.GetObject())["pics"].ToString()),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = (int)jsonCommentCount["response"].GetObject().GetNamedObject(CommentID).GetNamedNumber("comments")
                                });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<BoringPic>>(list, "boring_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 妹子图列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BoringPic>> GetMeiziPics(int pageNum)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<BoringPic> list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("girl_list.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_MEIZI, pageNum));

                    if (json != null)
                    {
                        List<BoringPic> list = new List<BoringPic>();
                        var posts = json["comments"];
                        if (posts != null)
                        {
                            JsonArray ja = posts.GetArray();
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentID);

                                list.Add(new BoringPic
                                {
                                    PicID = (j.GetObject())["comment_ID"].GetString(),
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["text_content"].GetString(),
                                    Urls = BoringPic.parse((j.GetObject())["pics"].ToString()),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = (int)jsonCommentCount["response"].GetObject().GetNamedObject(CommentID).GetNamedNumber("comments")
                                });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<BoringPic>>(list, "girl_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取段子/无聊图评论
        /// </summary>
        /// <returns></returns>
        public async Task<List<DuanComment>> GetDuanComments(string DuanID)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<DuanComment> list = await FileHelper.Current.ReadObjectAsync<List<DuanComment>>(string.Format("DuanComment-{0}.json", DuanID));
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(ServiceURL.URL_COMMENT_LIST + "comment-" + DuanID);

                    if (json != null)
                    {
                        List<DuanComment> list = new List<DuanComment>();

                        var postList = json["response"].GetArray();
                        if (postList != null && postList.GetArray().Count != 0)
                        {
                            var parentPosts = json["parentPosts"].GetObject();
                            foreach (var j in postList)
                            {
                                string postID = j.GetString();

                                var postItem = parentPosts.GetNamedObject(postID);

                                //string authorURL = postItem["author"].GetObject().GetNamedValue("avatar_url").ToString();
                                string authorURL;
                                try
                                {
                                    authorURL = postItem["author"].GetObject().GetNamedString("avatar_url");
                                }
                                catch (Exception)
                                {
                                    authorURL = "null";
                                }

                                list.Add(new DuanComment
                                {
                                    PostID = postItem.GetNamedString("post_id"),
                                    ThreadID = postItem.GetNamedString("thread_id"),
                                    Message = postItem.GetNamedString("message").Replace("<br />", ""),
                                    ParentID = postItem["parent_id"].ValueType == JsonValueType.String ? postItem.GetNamedString("parent_id") : "0",
                                    PostDate = postItem.GetNamedString("created_at"),
                                    AuthorName = postItem["author"].GetObject().GetNamedString("name"),
                                    AuthorAvatar = authorURL.Equals("null") ? "ms-appx:///Assets/Square150x150Logo.scale-400.png" : authorURL
                                });
                            }

                            // 添加评论回复字样
                            foreach (var l in list)
                            {

                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<DuanComment>>(list, string.Format("DuanComment-{0}.json", DuanID));
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取新鲜事评论
        /// </summary>
        /// <returns></returns>
        public async Task<List<FreshComment>> GetFreshComments(string DuanID)
        {
            try
            {
                if (NetworkManager.Current.Network == 4)  //无网络连接
                {
                    List<FreshComment> list = await FileHelper.Current.ReadObjectAsync<List<FreshComment>>(string.Format("FreshComment-{0}.json", DuanID));
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_FRESH_COMMENTS, DuanID)); //"comment-" + 

                    if (json != null)
                    {
                        List<FreshComment> list = new List<FreshComment>();

                        var post = json["post"].GetObject();
                        var comments = post["comments"].GetArray();
                        foreach (var c in comments)
                        {
                            // 特殊处理评论用户头像url
                            string authorURL;
                            try
                            {
                                authorURL = (c.GetObject()).GetNamedString("url");
                            }
                            catch (Exception)
                            {
                                authorURL = "null";
                            }

                            //var g = (c.GetObject())["parent"].ValueType == JsonValueType.String ? (c.GetObject())["parent"].ToString() : "0";
                            list.Add(new FreshComment
                            {
                                PostID = (c.GetObject()).GetNamedNumber("id").ToString(),
                                Message = (c.GetObject()).GetNamedString("content"),
                                ParentID = "0",
                                PostDate = (c.GetObject()).GetNamedString("date"),
                                AuthorName = (c.GetObject()).GetNamedString("name"),
                                AuthorAvatar = (authorURL.Equals("null")||authorURL.Equals("")) ? "ms-appx:///Assets/Square150x150Logo.scale-400.png" : authorURL
                            });
                        }

                        await FileHelper.Current.WriteObjectAsync<List<FreshComment>>(list, string.Format("FreshComment-{0}.json", DuanID));
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
