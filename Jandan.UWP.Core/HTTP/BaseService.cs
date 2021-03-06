﻿using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Jandan.UWP.Core.HTTP
{
    /// <summary>
    /// 访问HTTP服务器基础服务
    /// </summary>
    static class BaseService
    {
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据(string)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<string> SendGetRequest(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                Uri uri = new Uri(url);

                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }

        }        
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据(bytes)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<IBuffer> SendGetRequestAsBytes(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                Uri uri = new Uri(url);

                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsBufferAsync();
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据(string)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async static Task<string> SendPostRequest(string url, string body)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                mSent.Content = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json; charset=utf-8");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据(string)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async static Task<string> SendPostRequestUrlEncoded(string url, string body)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                mSent.Content = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded; charset=utf-8");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据(string)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async static Task<string> SendPostRequestUrlEncodedOfficial(string url, string body)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                mSent.Content = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded; charset=utf-8");

                HttpClient client = new HttpClient();

                var a = new Models.About();
                var client_name = $"Jandan UWP App V{a.VersionNumber}";
                client.DefaultRequestHeaders.Add("user-agent", client_name);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

                HttpResponseMessage response = await client.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据(string)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async static Task<string> SendPostRequestUrl(string url, string body)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                mSent.Content = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

                HttpClient client = new HttpClient();

                var a = new Models.About();
                var client_name = $"Jandan UWP App V{a.VersionNumber}";
                client.DefaultRequestHeaders.Add("user-agent", client_name);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                client.DefaultRequestHeaders.CacheControl.MaxAge = new TimeSpan(0);

                HttpResponseMessage response = await client.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
