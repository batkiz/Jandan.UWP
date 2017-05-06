using Jandan.UWP.Core.Models;
using Jandan.UWP.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Jandan.UWP.Control
{
    public sealed partial class CommentListControl : UserControl
    {
        /// <summary>
        /// 无聊图评论的View Model
        /// </summary>
        DuanCommentViewModel _dViewModel;

        public CommentListControl()
        {
            this.InitializeComponent();

            this.DataContext = _dViewModel = new DuanCommentViewModel();
        }

        public void SetFocus()
        {
            CommentSubmitButton.Focus(FocusState.Pointer);
        }

        public void Update(string id)
        {
            _dViewModel.Update(id);
        }

        public void ClearResponse()
        {
            _dViewModel.TextBoxComment = "";
        }

        private void DuanCommentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var d = e.ClickedItem as DuanComment;
            var user_name = d.AuthorName;

            _dViewModel.TextBoxComment = $"@{user_name}: ";
            _dViewModel.ParentId = d.PostID;
        }

        private async void CommentSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var response = _dViewModel.TextBoxComment;

            if (!response.StartsWith("@") && !string.IsNullOrEmpty(_dViewModel.ParentId))
            {
                _dViewModel.ParentId = "";
            }

            //var dia = new ContentDialog()
            //{
            //    Title = "提示",
            //    Content = new CommentSubmitDialogue(DataShareManager.Current.UserName, DataShareManager.Current.EmailAdd),
            //    PrimaryButtonText = "发送",
            //    SecondaryButtonText = "取消",
            //    FullSizeDesired = false,
            //    RequestedTheme = DataShareManager.Current.AppTheme
            //};
            //dia.PrimaryButtonClick += Dia_PrimaryButtonClick;

            //var result = await dia.ShowAsync();

            //if (result == ContentDialogResult.Primary)
            {
                //if (string.IsNullOrEmpty(DataShareManager.Current.UserName)||string.IsNullOrEmpty(DataShareManager.Current.EmailAdd))
                //{
                //    var dialog = new ContentDialog()
                //    {
                //        Title = "提示",
                //        Content = "请先在[设置]页面设置用户名和邮箱！",
                //        PrimaryButtonText = "确定",
                //        FullSizeDesired = false,
                //        RequestedTheme = DataShareManager.Current.AppTheme
                //    };

                //    dialog.PrimaryButtonClick += (_s, _e) => { };
                //    await dialog.ShowAsync();

                //    return;
                //}

                if (string.IsNullOrEmpty(DataShareManager.Current.AccessToken))
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "提示",
                        Content = "请先在[设置]页面设置第三方账号！",
                        PrimaryButtonText = "确定",
                        FullSizeDesired = false,
                        RequestedTheme = DataShareManager.Current.AppTheme
                    };

                    dialog.PrimaryButtonClick += (_s, _e) => { };
                    await dialog.ShowAsync();

                    return;
                }

                // 改为微博token格式
                //var message = $"message={response}&thread_id={_dViewModel.ThreadId}&parent_id={_dViewModel.ParentId}&author_name={DataShareManager.Current.UserName}&author_email={DataShareManager.Current.EmailAdd}";
                var message = $"message={response}&access_token={DataShareManager.Current.AccessToken}&thread_key={_dViewModel.ThreadKey}&parent_id={_dViewModel.ParentId}";

                var r = await _dViewModel.PostComment(message);                

                if (r != null)
                {
                    _dViewModel.TextBoxComment = "";

                    JsonObject j = new JsonObject();
                    if (JsonObject.TryParse(r, out j))
                    {
#if DEBUG
                        Debug.WriteLine(DateTime.Now.ToString() + j["response"].GetObject()["status"].ToString());
#endif
                        if (j["response"].GetObject()["status"].ToString().ToLower().Contains("approved"))
                        {
                            PopupMessage(1000, "评论成功");
                        }
                        else
                        {
                            PopupMessage(1000, "评论似乎没成功╮(╯_╰)╭");
                        }
                    }

                    string DuanID = _dViewModel.ThreadKey.Substring(_dViewModel.ThreadKey.IndexOf('-') + 1);
                    _dViewModel.Update(DuanID);
                }                              
            }            
        }

        private void Dia_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var csd = sender.Content as CommentSubmitDialogue;

            DataShareManager.Current.UserName = csd.UserName;
            DataShareManager.Current.EmailAdd = csd.Email;
        }

        private async void PopupMessage(int ms, string msg)
        {
            popText.Text = msg;

            double L = popTips.ActualWidth;
            double l = popText.ActualWidth;
            PopBorder.Margin = new Thickness((L - l) / 2, 0, 0, 0);

            popTips.IsOpen = true;   // 提示再按一次
            await Task.Delay(ms);  // 1000ms后关闭提示
            popTips.IsOpen = false;
        }
    }
}
