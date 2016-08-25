using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Jandan.Control
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
