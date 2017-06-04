using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Authentication.Web;

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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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


                        //******************************************
                        //await FileHelper.Current.WriteXmlObjectAsync<List<Fresh>>(list, "fresh.xml");
                        //******************************************



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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                                var author = (j.GetObject())["comment_author"].GetString();
                                var content = (j.GetObject())["comment_content"].GetString();
                                var date = (j.GetObject())["comment_date"].GetString();
                                var vote_pos = int.Parse(j.GetObject().GetNamedString("vote_positive"));
                                var vote_neg = int.Parse(j.GetObject().GetNamedString("vote_negative"));
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
                                    Author = author,
                                    Content = content,
                                    Date = date,
                                    VotePositive = vote_pos,
                                    VoteNegative = vote_neg,
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                                //var id = (j.GetObject())["id"] == null ? "000" : (j.GetObject())["id"].GetNumber().ToString();
                                string id = (j.GetObject())["comment_ID"] == null ? "000" : (j.GetObject())["comment_ID"].GetString();
                                string author = (j.GetObject())["comment_author"] == null ? "000" : (j.GetObject())["comment_author"].GetString();
                                string content = (j.GetObject())["text_content"] == null ? "000" : ((j.GetObject())["text_content"].GetString().Replace("\n", "").Replace("\r", ""));
                                var urls = BoringPic.ParseUrl((j.GetObject())["pics"].ToString(), ImageType.Original);
                                
                                if (string.Equals(Path.GetExtension(urls[0].URL).ToUpper(), ".GIF"))
                                {
                                    content = $"[GIF]\n{content}";
                                }

                                var thumbs = BoringPic.ParseUrl((j.GetObject())["pics"].ToString(), ImageType.Thumb);
                                var comment_count = "";
                                try
                                {
                                    comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{id}").GetNamedNumber("comments")).ToString();
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                list.Add(new BoringPic
                                {
                                    PicID = id,
                                    Author = author,
                                    Content = content,
                                    Urls = urls,
                                    Thumb = thumbs,
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                                    Urls = BoringPic.Parse((j.GetObject())["pics"].ToString()),
                                    Thumb = BoringPic.Parse((j.GetObject())["pics"].ToString(), true),
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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

                                var author = (j.GetObject())["comment_author"].GetString();
                                var content = (j.GetObject())["text_content"].GetString();

                                List<ImageItem> scr_list;
                                List<ImageItem> thumb_list;
                                BoringPic.ParseURL((j.GetObject())["comment_content"].GetString(), out scr_list, out thumb_list);

                                //var urls = BoringPic.parseHot((j.GetObject())["pics"].ToString());
                                //var thumb = BoringPic.parseHotThumb((j.GetObject())["pics"].ToString());
                                var date = (j.GetObject())["comment_date"].GetString();
                                var vote_pos = int.Parse(j.GetObject().GetNamedString("vote_positive"));
                                var vote_neg = int.Parse(j.GetObject().GetNamedString("vote_negative"));

                                list.Add(new BoringPic
                                {
                                    PicID = ID,
                                    Author = author,
                                    Content = content,
                                    Urls = scr_list,
                                    Thumb = thumb_list,
                                    Date = date,
                                    VotePositive = vote_pos,
                                    VoteNegative = vote_neg,
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
                {
                    List<DuanComment> list = await FileHelper.Current.ReadObjectAsync<List<DuanComment>>($"DuanComment-{DuanID}.json");
                    return list;
                }
                else
                {
                    //JsonObject json = await GetJson($"{ServiceURL.URL_COMMENT_LIST}comment-{DuanID}");
                    JsonObject json = await GetJson($"{ServiceURL.URL_COMMENT_LIST}{DuanID}");

                    if (json != null)
                    {
                        List<DuanComment> list = new List<DuanComment>();

                        var postList = json["tucao"].GetArray();
                        if (postList != null && postList.GetArray().Count != 0)
                        {
                            var floorLevel = 1;

                            foreach (var j in postList)
                            {
                                try
                                {
                                    string comment_id = (j.GetObject())["comment_ID"].GetString();
                                    string comment_post_id = (j.GetObject())["comment_post_ID"].GetString();
                                    string comment_author = (j.GetObject())["comment_author"].GetString();
                                    string comment_date = (j.GetObject())["comment_date"].GetString();
                                    string comment_content = (j.GetObject())["comment_content"].GetString();
                                    string comment_parent = (j.GetObject())["comment_parent"].GetString();
                                    string comment_reply_ID = (j.GetObject())["comment_reply_ID"].GetString();
                                    string vote_positive = (j.GetObject())["vote_positive"].GetString();
                                    string vote_negative = (j.GetObject())["vote_negative"].GetString();
                                    int is_tip_user = (int)(j.GetObject())["is_tip_user"].GetNumber();
                                    int is_jandan_user = (int)(j.GetObject())["is_jandan_user"].GetNumber();

                                    list.Add(new DuanComment
                                    {
                                        PostID = comment_id,
                                        ThreadID = comment_post_id,
                                        ThreadKey = comment_post_id,
                                        Message = comment_content,
                                        ParentID = comment_reply_ID,
                                        PostDate = comment_date,
                                        AuthorName = comment_author,
                                        AuthorAvatarUri = new Uri("ms-appx:///Icons/jandan-400.png"),
                                        Like = int.Parse(vote_positive),
                                        Dislike = int.Parse(vote_negative),
                                        OrderNumber = $"{floorLevel++}楼",
                                        VipUser = 10 * is_jandan_user + is_tip_user
                                    });
                                }
                                catch (Exception)
                                {
#if DEBUG
                                    Debug.WriteLine("吐槽JSON解析错误");
#endif
                                }                          
                            }

                            // 标记热门评论
                            var hotPosts = json["hot_tucao"].GetArray();
                            foreach (var h in hotPosts)
                            {
                                list?.ForEach((t) =>
                                {
                                    if (t.PostID == (h.GetObject())["comment_ID"].GetString())
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
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
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
                        var floorLevel = 1;
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

                            var id = (c.GetObject()).GetNamedNumber("id").ToString();
                            var msg = (c.GetObject()).GetNamedString("content");
                            var date = (c.GetObject()).GetNamedString("date");
                            var author = (c.GetObject()).GetNamedString("name");
                            var like = (int)(c.GetObject()).GetNamedNumber("vote_positive");
                            var dislike = (int)(c.GetObject()).GetNamedNumber("vote_negative");

                            list.Add(new FreshComment
                            {
                                PostID = id,
                                Message = msg,
                                ParentID = "0",
                                PostDate = date,
                                AuthorName = author,
                                AuthorAvatarUri = new Uri((authorURL.Equals("null") || authorURL.Equals("")) ? "ms-appx:///Icons/jandan-400.png" : authorURL),
                                Like = like,
                                Dislike = dislike,
                                IsHot = (like > 20) ? ((like / (double)(dislike + 1) > 1.5) ? true : false) : false,
                                OrderNumber = $"{floorLevel++}楼"
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
        
        private void Printlog(string info)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + info);
#endif
        }
        public async Task GetAccessTokenAsync(string s)
        {
            string authUrl = "";

            switch (s)
            {
                case "微博":
                    authUrl = ServiceURL.URL_DUOSHUO_WEIBO;
                    break;
                case "QQ":
                    authUrl = ServiceURL.URL_DUOSHUO_QQ;
                    break;
                case "百度":
                    authUrl = ServiceURL.URL_DUOSHUO_BAIDU;
                    break;
                case "豆瓣":
                    authUrl = ServiceURL.URL_DUOSHUO_DOUBAN;
                    break;
                case "人人":
                    authUrl = ServiceURL.URL_DUOSHUO_RENREN;
                    break;
                case "开心网":
                    authUrl = ServiceURL.URL_DUOSHUO_KAIXIN;
                    break;
                default:
                    break;
            }
            // 新浪微博授权地址
            //string authUrl = $"https://jandan.duoshuo.com/login/weibo/?sso=1&redirect_uri=http://jandan.net/";
            Uri wbAuthUri = new Uri(authUrl);

            // 回调地址
            string cbUri = @"http://jandan.net";
            Uri callbackUri = new Uri(cbUri);

            // 获取授权
            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, wbAuthUri, callbackUri);

            // 处理结果
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                string cburi = result.ResponseData;
                // 取得授权码
                // code是附加在回调URI后，以?code=xxxxxxxxxxxxxx的形式出现，作为URI的查询字符串
                string code = cburi.Substring(cburi.IndexOf('=') + 1);
                                
                Printlog($"回调URI：{cburi}\n授权码：{code}");

                try
                {
                    string url = @"http://api.duoshuo.com/oauth2/access_token";
                    string msg = $"client_id=jandan&code={code}";
                    string returned_msg = await BaseService.SendPostRequestUrl(url, msg);

                    if (returned_msg != null)
                    {
                        Printlog("请求token数据成功");
                        var accessInfo = JsonObject.Parse(returned_msg);

                        var access_token = accessInfo["access_token"].GetString();
                        DataShareManager.Current.UpdateAccessToken(access_token);
                        //DataShareManager.Current.AccessToken = access_token;

                        var user_id = accessInfo["user_id"].GetString();
                        var user_info = await GetJson($"http://api.duoshuo.com/users/profile.json?user_id={user_id}");
                        DataShareManager.Current.UpdateUserId3rd(user_id);

                        if (user_info != null)
                        {
                            var response = user_info["response"].GetObject();
                            //DataShareManager.Current.ThirdPartyUserName = response["name"].GetString();
                            DataShareManager.Current.UpdateUserName3rd(response["name"].GetString());
                        }
                    }                    
                }
                catch (Exception)
                {

                }
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                Printlog("错误：" + result.ResponseErrorDetail.ToString());
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                Printlog("你取消了操作。");
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
                string returned_msg = await BaseService.SendPostRequestUrl(url, comment);

                return returned_msg;
            }
            catch (Exception)
            {
                return "Failed.";
            }
        }

        public async Task<string> PostFreshComment(string comment)
        {
            try
            {
                string url = ServiceURL.URL_PUSH_COMMENT;
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
