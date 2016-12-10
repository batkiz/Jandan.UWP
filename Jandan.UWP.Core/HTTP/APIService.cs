using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Jandan.UWP.Core.HTTP
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    if (pageNum == 1)
                    {
                        List<Fresh> list = await FileHelper.Current.ReadObjectAsync<List<Fresh>>("fresh_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
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
                                var id = (j.GetObject())["id"] == null ? "000" : (j.GetObject())["id"].GetNumber().ToString();
                                var title = (j.GetObject())["title"] == null ? "" : (j.GetObject())["title"].GetString();
                                var author = (j.GetObject())["author"] == null ? new Authors() : Authors.parse((j.GetObject())["author"].GetObject().ToString());
                                var tag = (j.GetObject())["tags"] == null ? new List<Tags>() : Tags.parse((j.GetObject())["tags"].ToString());
                                var comment_count = (j.GetObject())["comment_count"] == null ? "000" : (j.GetObject())["comment_count"].GetNumber().ToString();
                                var date = (j.GetObject())["date"] == null ? "1111-11-11 11:11:11" : (j.GetObject())["date"].GetString();
                                var url = (j.GetObject())["url"] == null ? "" : (j.GetObject())["url"].GetString();
                                var thumb = (j.GetObject())["custom_fields"] == null ? "ms-appx:///Assets/Square150x150Logo.scale-200.png" : ((j.GetObject())["custom_fields"].GetObject())["thumb_c"].GetArray().GetStringAt(0);

                                list.Add(new Fresh
                                {
                                    ID = id,
                                    Title = title,
                                    Author = author,
                                    Tag = tag,
                                    Comment_count = comment_count,
                                    Thumb_c = thumb,
                                    Date = date,
                                    Url = url
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    FreshDetail list = await FileHelper.Current.ReadObjectAsync<FreshDetail>($"freshDetail-{fresh.ID}.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(string.Format(ServiceURL.URL_FRESH_NEWS_DETAIL, fresh.ID));

                    if (json != null)
                    {

                        var post = json["post"];
                        string htmlContent = (post.GetObject())["content"].GetString();

                        if (fresh.Tag == null)
                        {
                            var tag = Tags.parse((post.GetObject())["tags"].ToString());
                            fresh.Tag = tag;
                        }                        

                        FreshDetail list = new FreshDetail() { FreshInfo = fresh, FreshContentSlim = htmlContent, FreshContentEx = htmlContent };
                        await FileHelper.Current.WriteObjectAsync<FreshDetail>(list, $"freshDetail-{fresh.ID}.json");
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    if (pageNum == 1)
                    {
                        List<Duan> list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("duan_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
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

                            string CommentIDList = "";
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                CommentIDList = $"{CommentIDList},{CommentID}";
                            }

                            JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {                                
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                list.Add(new Duan
                                {
                                    DuanID = ID,
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["comment_content"].GetString(),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = comment_count
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    if (pageNum == 1)
                    {
                        List<BoringPic> list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("boring_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
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

                            string CommentIDList = "";
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                CommentIDList = $"{CommentIDList},{CommentID}";
                            }

                            JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var author = (j.GetObject())["comment_author"].GetString();
                                var comment_count = "";
                                try
                                {
                                    comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                list.Add(new BoringPic
                                {
                                    PicID = ID,
                                    Author = author,
                                    Content = (j.GetObject())["text_content"].GetString().Replace("\n", "").Replace("\r", ""),
                                    Urls = BoringPic.parse((j.GetObject())["pics"].ToString()),
                                    Thumb = BoringPic.parseThumb((j.GetObject())["pics"].ToString()),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = comment_count
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    if (pageNum == 1)
                    {
                        List<BoringPic> list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("girl_list.json");
                        return list;
                    }
                    else
                    {
                        return null;
                    }
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

                            string CommentIDList = "";
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                CommentIDList = $"{CommentIDList},{CommentID}";
                            }

                            JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                list.Add(new BoringPic
                                {
                                    PicID = ID,
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["text_content"].GetString().Replace("\n", "").Replace("\r", ""),
                                    Urls = BoringPic.parse((j.GetObject())["pics"].ToString()),
                                    Thumb = BoringPic.parseThumb((j.GetObject())["pics"].ToString()),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = comment_count
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
        /// 热门无聊图列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<BoringPic>> GetHotPics()
        {
            try
            {
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    List<BoringPic> list = await FileHelper.Current.ReadObjectAsync<List<BoringPic>>("hot_pics_list.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(ServiceURL.URL_HOTPICS);

                    if (json != null)
                    {
                        List<BoringPic> list = new List<BoringPic>();
                        var posts = json["comments"];
                        if (posts != null)
                        {
                            JsonArray ja = posts.GetArray();

                            string CommentIDList = "";
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                CommentIDList = $"{CommentIDList},{CommentID}";
                            }

                            JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                list.Add(new BoringPic
                                {
                                    PicID = ID,
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["text_content"].GetString(),
                                    Urls = BoringPic.parseHot((j.GetObject())["pics"].ToString()),
                                    Thumb = BoringPic.parseHotThumb((j.GetObject())["pics"].ToString()),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = comment_count
                                });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync<List<BoringPic>>(list, "hot_pics_list.json");
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
        /// 热门无聊图列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Duan>> GetHotDuan()
        {
            try
            {
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    List<Duan> list = await FileHelper.Current.ReadObjectAsync<List<Duan>>("hot_duan_list.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(ServiceURL.URL_HOTDUAN);

                    if (json != null)
                    {
                        List<Duan> list = new List<Duan>();
                        var posts = json["comments"];
                        if (posts != null)
                        {
                            JsonArray ja = posts.GetArray();

                            string CommentIDList = "";
                            foreach (var j in ja)
                            {
                                string CommentID = "comment-" + (j.GetObject())["comment_ID"].GetString();
                                CommentIDList = $"{CommentIDList},{CommentID}";
                            }

                            JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                list.Add(new Duan
                                {
                                    DuanID = ID,
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = (j.GetObject())["text_content"].GetString(),
                                    Date = (j.GetObject())["comment_date"].GetString(),
                                    VotePositive = int.Parse(j.GetObject().GetNamedString("vote_positive")),
                                    VoteNegative = int.Parse(j.GetObject().GetNamedString("vote_negative")),
                                    CommentCount = comment_count
                                });
                            }
                        }
                        await FileHelper.Current.WriteObjectAsync(list, "hot_duan_list.json");
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
        public async Task<List<BestFreshComment>> GetHotComments()
        {
            try
            {
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    List<BestFreshComment> list = await FileHelper.Current.ReadObjectAsync<List<BestFreshComment>>("BestFreshComment.json");
                    return list;
                }
                else
                {
                    JsonObject json = await GetJson(ServiceURL.URL_HOTCOMM); //"comment-" + 

                    if (json != null)
                    {
                        List<BestFreshComment> list = new List<BestFreshComment>();
                        
                        var comments = json["comments"].GetArray();
                        foreach (var c in comments)
                        {
                            var obj = c.GetObject();
                            var post = obj.GetNamedObject("post");
                            
                            list.Add(new BestFreshComment
                            {
                                PostID = obj.GetNamedString("comment_post_ID"),
                                CommentID = obj.GetNamedString("comment_ID"),
                                Content = obj.GetNamedString("comment_content"),
                                AuthorName = obj.GetNamedString("comment_author"),
                                PostDate = obj.GetNamedString("comment_date"),
                                Like = int.Parse(obj.GetNamedString("vote_positive")),
                                Dislike = int.Parse(obj.GetNamedString("vote_negative")),
                                Title = post.GetNamedString("post_title"),
                                FreshNews = new FreshDetail
                                {
                                    FreshInfo = new Fresh
                                    {
                                        Author = new Authors { Name = post.GetNamedString("post_author") },
                                        Date = post.GetNamedString("post_date"),
                                        ID = post.GetNamedNumber("ID").ToString(),
                                        Title = post.GetNamedString("post_title"),
                                        Comment_count = post.GetNamedString("comment_count"),
                                        Url = post.GetNamedString("guid")
                                    },
                                    FreshContentEx = post.GetNamedString("post_content"),
                                    FreshContentSlim = post.GetNamedString("post_content")
                                }
                            });
                        }

                        await FileHelper.Current.WriteObjectAsync<List<BestFreshComment>>(list, "BestFreshComment.json");
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
                {
                    List<DuanComment> list = await FileHelper.Current.ReadObjectAsync<List<DuanComment>>($"DuanComment-{DuanID}.json");
                    return list;
                }
                else
                {                    
                    JsonObject json = await GetJson($"{ServiceURL.URL_COMMENT_LIST}comment-{DuanID}");

                    if (json != null)
                    {
                        List<DuanComment> list = new List<DuanComment>();

                        var postList = json["response"].GetArray();
                        if (postList != null && postList.GetArray().Count != 0)
                        {
                            var parentPosts = json["parentPosts"].GetObject();
                            var floorLevel = 1;
                            foreach (var j in postList)
                            {
                                string postID = j.GetString();

                                try
                                {
                                    var postItem = parentPosts.GetNamedObject(postID);
                                    // 获取评论用户图像URL
                                    string authorURL;
                                    try
                                    {
                                        var jsonAuthor = postItem["author"].GetObject();
                                        authorURL = jsonAuthor["avatar_url"].ValueType == JsonValueType.String ? jsonAuthor.GetNamedString("avatar_url") : "null";
                                    }
                                    catch (Exception)
                                    {
                                        authorURL = "null";
                                    }

                                    // 获取评论列表
                                    var timestr = postItem.GetNamedString("created_at");
                                    CultureInfo cultureInfo = new CultureInfo("en-US");
                                    string format = "yyyy-MM-ddTHH:mm:sszzz";
                                    DateTime datetime = DateTime.ParseExact(timestr, format, cultureInfo);
                                    
                                    list.Add(new DuanComment
                                    {
                                        PostID = postItem.GetNamedString("post_id"),
                                        ThreadID = postItem.GetNamedString("thread_id"),
                                        Message = postItem.GetNamedString("message"),
                                        ParentID = postItem["parent_id"].ValueType == JsonValueType.String ? postItem.GetNamedString("parent_id") : "0",
                                        PostDate = datetime.ToString(),
                                        AuthorName = postItem["author"].GetObject().GetNamedString("name"),
                                        AuthorAvatarUri = new Uri((authorURL.Equals("null") || authorURL.Equals("")) ? "ms-appx:///Icons/jandan-400.png" : authorURL),
                                        Like = (int)postItem.GetNamedNumber("likes"),
                                        Dislike = (int)postItem.GetNamedNumber("dislikes"),
                                        OrderNumber = $"{floorLevel++}楼"
                                    });
                                }
                                catch (System.Exception)
                                {
                                    Debug.WriteLine("存在以下Post ID无对应评论：" + postID);
                                }                                
                            }

                            // 标记热门评论
                            var hotPosts = json["hotPosts"].GetArray();
                            foreach (var h in hotPosts)
                            {
                                list?.ForEach((t) =>
                                {
                                    if (t.PostID == h.GetString())
                                    {
                                        t.IsHot = true;
                                    }
                                });
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
                if (ConnectivityManager.Current.Network == NetworkType.NotConnected)  //无网络连接
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
                                AuthorAvatarUri = new Uri((authorURL.Equals("null")||authorURL.Equals("")) ? "ms-appx:///Icons/jandan-400.png" : authorURL)
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

        public async Task<string> Vote(string ID, bool isLike)
        {
            int like = isLike ? 1 : 0;

            try
            {
                string url = string.Format(ServiceURL.URL_VOTE, like);
                string body = $"ID={ID}";
                string returned_msg = await BaseService.SendPostRequestUrlEncodedOfficial(url, body);

                return returned_msg;
            }
            catch (Exception)
            {
                return "Failed.";
            }
        }

        public async Task<string> PostComment(string comment)
        {
            try
            {
                string url = ServiceURL.URL_PUSH_DUAN_COMMENT;
                string returned_msg = await BaseService.SendPostRequestUrlEncodedOfficial(url, comment);

                return returned_msg;
            }
            catch (Exception)
            {
                return "Failed.";
            }
        }
    }
}
