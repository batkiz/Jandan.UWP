﻿using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.Tools;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
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
        private static string _local_path = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;

        /// <summary>
        /// 新鲜事列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Fresh>> GetFresh(int pageNum)
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
                    JsonObject json = await GetJson(string.Format(ServiceURL.API_GET_FRESH_NEWS, pageNum));

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
                                var excerpt = (j.GetObject())["excerpt"] == null ? "" : (j.GetObject())["excerpt"].GetString();
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
                                    Excerpt = excerpt,
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
        public static async Task<FreshDetail> GetFreshDetail(Fresh fresh)
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
                    JsonObject json = await GetJson(string.Format(ServiceURL.API_GET_FRESH_NEWS_DETAIL, fresh.ID));

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
        public static async Task<List<Duan>> GetDuan(int pageNum)
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
                    JsonObject json = await GetJson(string.Format(ServiceURL.API_GET_DUANZI, pageNum));

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

                            //JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {                                
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var author = (j.GetObject())["comment_author"].GetString();
                                var content = (j.GetObject())["comment_content"].GetString();
                                var con_type = "text";

                                var s = Regex.Match(content, "<img src=\"(.+)\" />");
                                if (s.Length != 0)
                                {
                                    content = Regex.Replace(content, "<img src=\"(.+)\" />", "$1");
                                    con_type = "image";
                                }

                                var date = (j.GetObject())["comment_date"].GetString();
                                var vote_pos = int.Parse(j.GetObject().GetNamedString("vote_positive"));
                                var vote_neg = int.Parse(j.GetObject().GetNamedString("vote_negative"));
                                var comment_count = "";
                                try
                                {
                                    //comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                    comment_count = j.GetObject().GetNamedString("sub_comment_count");
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
                                    CommentCount = comment_count,
                                    ContentType = con_type
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
        public static async Task<List<BoringPic>> GetBoringPics(int pageNum)
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
                    JsonObject json = await GetJson(string.Format(ServiceURL.API_GET_BORING_PICTURE, pageNum));

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

                            //JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                //var id = (j.GetObject())["id"] == null ? "000" : (j.GetObject())["id"].GetNumber().ToString();
                                string id = (j.GetObject())["comment_ID"] == null ? "000" : (j.GetObject())["comment_ID"].GetString();                                
                                string author = (j.GetObject())["comment_author"] == null ? "000" : (j.GetObject())["comment_author"].GetString();
                                string content = (j.GetObject())["text_content"] == null ? "000" : ((j.GetObject())["text_content"].GetString().Replace("\n", "").Replace("\r", ""));
                                
                                // 从Json读取到的url不一定可靠，再用正则表达式做一次检查
                                var url_org = BoringPic.ParseUrl((j.GetObject())["pics"].ToString(), ImageType.Original);
                                List<ImageItem> urls = new List<ImageItem>();
                                foreach (var u in url_org)
                                {
                                    var r = Regex.Matches(u.URL, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                    foreach (var rr in r)
                                    {
                                        urls.Add(new ImageItem(rr.ToString()));
                                    }
                                }

                                // 如果图片里包含GIF，则在content中增加GIF标签
                                if (urls.Exists((t) =>
                                {
                                    if (t.URL.ToUpper().Contains("GIF")) { return true; }
                                    else { return false; }
                                }))
                                {
                                    content = $"[GIF]\n{content}";
                                }

                                var thumbs = BoringPic.ParseUrl((j.GetObject())["pics"].ToString(), ImageType.Thumb);
                                var comment_count = "";
                                try
                                {
                                    comment_count = j.GetObject().GetNamedString("sub_comment_count");
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
        public static async Task<List<BoringPic>> GetMeiziPics(int pageNum)
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
                    JsonObject json = await GetJson(string.Format(ServiceURL.API_GET_MEIZI, pageNum));

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

                            //JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    //comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                    comment_count = j.GetObject().GetNamedString("sub_comment_count");
                                }
                                catch (Exception)
                                {
                                    comment_count = "";
                                }

                                string content = (j.GetObject())["text_content"] == null ? "000" : ((j.GetObject())["text_content"].GetString().Replace("\n", "").Replace("\r", ""));
                                // 从Json读取到的url不一定可靠，再用正则表达式做一次检查
                                var url_org = BoringPic.ParseUrl((j.GetObject())["pics"].ToString(), ImageType.Original);
                                List<ImageItem> urls = new List<ImageItem>();
                                foreach (var u in url_org)
                                {
                                    var r = Regex.Matches(u.URL, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                    foreach (var rr in r)
                                    {
                                        urls.Add(new ImageItem(rr.ToString()));
                                    }
                                }
                                // 如果图片里包含GIF，则在content中增加GIF标签
                                if (urls.Exists((t) =>
                                {
                                    if (t.URL.ToUpper().Contains("GIF")) { return true; }
                                    else { return false; }
                                }))
                                {
                                    content = $"[GIF]\n{content}";
                                }

                                list.Add(new BoringPic
                                {
                                    PicID = ID,
                                    Author = (j.GetObject())["comment_author"].GetString(),
                                    Content = content,
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
        public static async Task<List<BoringPic>> GetHotPics()
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
                    JsonObject json = await GetJson(ServiceURL.API_GET_HOTPICS);

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

                            //JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    //comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                    comment_count = j.GetObject().GetNamedString("sub_comment_count");
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
        public static async Task<List<Duan>> GetHotDuan()
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
                    JsonObject json = await GetJson(ServiceURL.API_GET_HOTDUAN);

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

                            //JsonObject jsonCommentCount = await GetJson(ServiceURL.URL_COMMENT_COUNTS + CommentIDList);
                            foreach (var j in ja)
                            {
                                string ID = (j.GetObject())["comment_ID"].GetString();
                                var comment_count = "";
                                try
                                {
                                    //comment_count = ((int)jsonCommentCount["response"].GetObject().GetNamedObject($"comment-{ID}").GetNamedNumber("comments")).ToString();
                                    comment_count = j.GetObject().GetNamedString("sub_comment_count");
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
        public static async Task<List<BestFreshComment>> GetHotComments()
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
                    JsonObject json = await GetJson(ServiceURL.API_GET_HOTCOMM); //"comment-" + 

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
        public static async Task<List<Tucao>> GetTucao(string DuanID)
        {
            try
            {
                if (!ConnectivityHelper.isInternetAvailable)  //无网络连接
                {
                    List<Tucao> list = await FileHelper.Current.ReadObjectAsync<List<Tucao>>($"DuanComment-{DuanID}.json");
                    return list;
                }
                else
                {
                    //JsonObject json = await GetJson($"{ServiceURL.URL_COMMENT_LIST}comment-{DuanID}");
                    JsonObject json = await GetJson($"{ServiceURL.API_POST_COMMENT_LIST}{DuanID}");

                    if (json != null)
                    {
                        List<Tucao> list = new List<Tucao>();

                        var postList = json["tucao"].GetArray();

                        // 判断吐槽是否还有下一页，并添加到当前评论列表中
                        var has_next_page = json["has_next_page"].GetBoolean();
                        while (has_next_page)
                        {
                            // 首先获取当前列表最后一项comment_ID
                            var last_comment_ID = (postList[postList.Count - 1].GetObject())["comment_ID"].GetString();

                            // 获取下一页评论
                            JsonObject json2 = await GetJson($"{ServiceURL.API_POST_COMMENT_LIST}{DuanID}/n/{last_comment_ID}");

                            // 与当前postList合并
                            foreach (var j in json2["tucao"].GetArray())
                            {
                                postList.Add(j);
                            }

                            has_next_page = json2["has_next_page"].GetBoolean();
                        }

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

                                    // 正则表达式做一次检查, 是否存在图片url
                                    string comment_type = "text";
                                    List<ImageItem> urls = new List<ImageItem>();
                                    var r = Regex.Matches(comment_content, @"([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                                    if (r.Count != 0)
                                    {
                                        //foreach (var rr in r)
                                        //{
                                        //    urls.Add(new ImageItem(rr.ToString()));
                                        //}
                                        List<ImageItem> scr_list;
                                        List<ImageItem> thumb_list;
                                        BoringPic.ParseURL(comment_content, out scr_list, out thumb_list);

                                        urls = scr_list;
                                        comment_type = "text_with_image";
                                    }
                                    

                                    string comment_parent = (j.GetObject())["comment_parent"].GetString();
                                    string comment_reply_ID = (j.GetObject())["comment_reply_ID"].GetString();
                                    string vote_positive = (j.GetObject())["vote_positive"].GetString();
                                    string vote_negative = (j.GetObject())["vote_negative"].GetString();
                                    int is_tip_user = (int)(j.GetObject())["is_tip_user"].GetNumber();
                                    int is_jandan_user = (int)(j.GetObject())["is_jandan_user"].GetNumber();

                                    list.Add(new Tucao
                                    {
                                        PostID = comment_id,
                                        ThreadID = comment_post_id,
                                        ThreadKey = comment_post_id,
                                        Message = comment_content,
                                        ContentType = comment_type,
                                        Urls = urls,
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
                        await FileHelper.Current.WriteObjectAsync<List<Tucao>>(list, string.Format("DuanComment-{0}.json", DuanID));
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
        public static async Task<List<FreshComment>> GetFreshComments(string DuanID)
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
                    JsonObject json = await GetJson(string.Format(ServiceURL.API_GET_FRESH_COMMENTS, DuanID)); //"comment-" + 

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

        public static async Task<string> Vote(string ID, bool isLike)
        {
            int like = isLike ? 1 : 0;

            try
            {
                string url = string.Format(ServiceURL.API_POST_VOTE, like);
                string body = $"ID={ID}";
                string returned_msg = await BaseService.SendPostRequestUrlEncodedOfficial(url, body);

                return returned_msg;
            }
            catch (Exception)
            {
                return "Failed.";
            }
        }

        public static async Task<string> PostComment(string comment)
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

        public static async Task<string> PostFreshComment(string comment)
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
