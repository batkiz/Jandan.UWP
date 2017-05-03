using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Jandan.UWP.Core.Tools
{
    public class FileHelper
    {
        private static FileHelper _current;
        public static FileHelper Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new FileHelper();
                }
                return _current;
            }
        }

        private StorageFolder _local_folder;
        private List<StorageFolder> _local_cache_folders;

        public FileHelper()
        {
            _local_folder = ApplicationData.Current.LocalCacheFolder;
            _local_cache_folders = new List<StorageFolder>();
            Init();
        }

        private async void Init()
        {
            var folder_1 = await _local_folder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
            var folder_2 = await _local_folder.CreateFolderAsync("data_cache", CreationCollisionOption.OpenIfExists);
            var folder_3 = await _local_folder.CreateFolderAsync("favourite", CreationCollisionOption.OpenIfExists);

            _local_cache_folders.Add(folder_1);
            _local_cache_folders.Add(folder_2);
            //_local_cache_folders.Add(folder_3);
        }
        public async Task WriteObjectAsync<T>(T obj, string filename)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + $"写入Json数据：{filename}");
#endif
            try
            {
                var folder = await _local_folder.CreateFolderAsync("data_cache", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    DataContractJsonSerializer serizlizer = new DataContractJsonSerializer(typeof(T));
                    serizlizer.WriteObject(stream, obj);

                    stream.Flush();
                }
            }
            catch(Exception e)
            {
#if DEBUG
                Debug.WriteLine(DateTime.Now.ToString() + " " + e.Message);
#endif
            }
        }
        public async Task<T> ReadObjectAsync<T>(string filename) where T : class
        {
            try
            {
                var folder = await _local_folder.CreateFolderAsync("data_cache", CreationCollisionOption.OpenIfExists);
                using (var data = await folder.OpenStreamForReadAsync(filename))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                    return serializer.ReadObject(data) as T;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task WriteXmlObjectAsync<T>(T obj, string filename)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + $"写入Xml数据：{filename}");
#endif
            try
            {
                var folder = await _local_folder.CreateFolderAsync("favourite", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream raStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                    {
                        // 创建序列化对象写入数据
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(outStream.AsStreamForWrite(), obj);
                        await outStream.FlushAsync();
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine(DateTime.Now.ToString() + " " + e.Message);
#endif
            }
        }
        public async Task<T> ReadXmlObjectAsync<T>(string filename)
        {
#if DEBUG
            Debug.WriteLine(DateTime.Now.ToString() + " " + $"读取Xml数据：{filename}");
#endif
            try
            {
                T sessionState_ = default(T);

                var folder = await _local_folder.CreateFolderAsync("favourite", CreationCollisionOption.OpenIfExists);
                var file = await folder.GetFileAsync(filename);
                if (file == null)
                {
                    return sessionState_;
                }
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                    sessionState_ = (T)serializer.ReadObject(inStream.AsStreamForRead());
                }
                return sessionState_;
            }
            catch
            {
                return default(T);
            }
        }

        public async Task SaveImageAsync(WriteableBitmap image, string filename)
        {
            try
            {
                if (image == null)
                {
                    return;
                }
                Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                if (filename.EndsWith("jpg"))
                    BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                else if (filename.EndsWith("png"))
                    BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                else if (filename.EndsWith("bmp"))
                    BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                else if (filename.EndsWith("tiff"))
                    BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                else if (filename.EndsWith("gif"))
                    BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
                var folder = await _local_folder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                    Stream pixelStream = image.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)image.PixelWidth,
                              (uint)image.PixelHeight,
                              96.0,
                              96.0,
                              pixels);
                    await encoder.FlushAsync();
                }
            }
            catch
            {

            }
        }

        public async Task DeleteCacheFile()
        {
            try
            {
                //StorageFolder folder = await _local_folder.GetFolderAsync("images_cache");
                foreach (var folder in _local_cache_folders)
                {
                    if (folder != null)
                    {
                        IReadOnlyCollection<StorageFile> files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery);
                        //files.ToList().ForEach(async (f) => await f.DeleteAsync(StorageDeleteOption.PermanentDelete));
                        List<IAsyncAction> list = new List<IAsyncAction>();
                        foreach (var f in files)
                        {
                            list.Add(f.DeleteAsync(StorageDeleteOption.PermanentDelete));
                        }
                        List<Task> list2 = new List<Task>();
                        list.ForEach((t) => list2.Add(t.AsTask()));

                        await Task.Run(() => { Task.WaitAll(list2.ToArray()); });
                    }
                }                
            }
            catch
            {

            }
        }
        public async Task<double> GetCacheSize()
        {
            try
            {
                //StorageFolder folder = await _local_folder.GetFolderAsync("images_cache");
                double size = 0; BasicProperties p;
                foreach (var folder in _local_cache_folders)
                {
                    var files = await folder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery);
                    foreach (var f in files)
                    {
                        p = await f.GetBasicPropertiesAsync();
                        size += p.Size;
                    }
                }                
                return size;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> CacheExist(string filename)
        {
            try
            {
                var f = await _local_folder.TryGetItemAsync("images_cache");
                if (f != null)
                {
                    var f2 = await (f as StorageFolder).TryGetItemAsync(filename);
                    if (f2 == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public string GetFormatSize(double size)
        {
            if (size < 1024)
            {
                return size + "byte";
            }
            else if (size < 1024 * 1024)
            {
                return Math.Round(size / 1024, 2) + "KB";
            }
            else if (size < 1024 * 1024 * 1024)
            {
                return Math.Round(size / 1024 / 1024, 2) + "MB";
            }
            else
            {
                return Math.Round(size / 1024 / 1024 / 2014, 2) + "GB";
            }
        }
    }
}
