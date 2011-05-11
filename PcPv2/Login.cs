using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PcPv2
{
    public partial class Login : Form
    {
        public string username = "";
        public string password = "";
        public bool cancelled = false;

        // save the default user name for display
        public Login(string defaultUser)
        {
            username = defaultUser;
            InitializeComponent();
        }

        // accept the login info via a button
        private void btnLogin_Click(object sender, EventArgs e)
        {
            accept();
        }

        // accept the login info via pressing the enter key
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                accept();
            }
        }

        // save the username and password and close the dialog
        private void accept()
        {
            username = txtUsername.Text;
            password = txtPassword.Text;
            Close();
        }

        // cancel the login process
        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancelled = true;
            Close();
        }

        // load up the login prompt and fill in a username if one exists
        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (username != "")
            {
                txtUsername.Text = username;
                txtPassword.Focus();
            }
        }
    }
}
