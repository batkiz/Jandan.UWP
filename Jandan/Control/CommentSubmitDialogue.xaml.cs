using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Jandan.UWP.Control
{
    public sealed partial class CommentSubmitDialogue : UserControl
    {
        public CommentSubmitDialogue()
        {
            this.InitializeComponent();
        }

        public CommentSubmitDialogue(string user_name, string email)
        {
            this.InitializeComponent();

            this.textBoxUserName.Text = user_name == null ? "" : user_name;
            this.textBoxEmail.Text = email == null ? "" : email;
        }

        public string UserName
        { get { return this.textBoxUserName.Text; } }

        public string Email
        { get { return this.textBoxEmail.Text; } }
    }
}
