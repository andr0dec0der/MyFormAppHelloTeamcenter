using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Teamcenter.ClientX;
using Teamcenter.Services.Strong.Core;
using Teamcenter.Services.Strong.Core._2006_03.Session;
using System.Threading;
using Teamcenter.Schemas.Soa._2006_03.Exceptions;

namespace HelloTeamcenter.form
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void passText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loginButton.PerformClick();
                loginButton.Enabled = false;
            }
        }

        internal string getUserName()
        {
            return userNameText.Text;
        }

        internal string getPassword()
        {
            return passText.Text;
        }
    }
}
