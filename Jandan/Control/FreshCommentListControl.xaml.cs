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
    public sealed partial class FreshCommentListControl : UserControl
    {
        /// <summary>
        /// 无聊图评论的View Model
        /// </summary>
        FreshCommentViewModel _dViewModel;

        public FreshCommentListControl()
        {
            this.InitializeComponent();

            this.DataContext = _dViewModel = new FreshCommentViewModel();
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
            var d = e.ClickedItem as FreshComment;
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

            var dia = new ContentDialog()
            {
                Title = "提示",
                Content = new CommentSubmitDialogue(DataShareManager.Current.UserName, DataShareManager.Current.EmailAdd),
                PrimaryButtonText = "发送",
                SecondaryButtonText = "取消",
                FullSizeDesired = false,
                RequestedTheme = DataShareManager.Current.AppTheme
            };
            dia.PrimaryButtonClick += Dia_PrimaryButtonClick;

            var result = await dia.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var message = $"message={response}&thread_id={_dViewModel.ThreadId}&parent_id={_dViewModel.ParentId}&author_name={DataShareManager.Current.UserName}&author_email={DataShareManager.Current.EmailAdd}";

                var r = await _dViewModel.PostComment(message);

                _dViewModel.TextBoxComment = "";

                JsonObject j = new JsonObject();
                if (JsonObject.TryParse(r, out j))
                {
                    Debug.WriteLine(DateTime.Now.ToString() + "评论成功！");
                }                
            }            
        }

        private void Dia_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var csd = sender.Content as CommentSubmitDialogue;

            DataShareManager.Current.UserName = csd.UserName;
            DataShareManager.Current.EmailAdd = csd.Email;
        }
    }
}
