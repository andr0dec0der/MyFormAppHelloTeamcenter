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
using Teamcenter.Schemas.Soa._2006_03.Exceptions;
using Teamcenter.Soa.Client.Model.Strong;
using Teamcenter.Soa.Client.Model;
using Teamcenter.Hello;
using Teamcenter.Soa.Exceptions;
using System.Collections;

namespace HelloTeamcenter.form
{
    public partial class ExampleForm : System.Windows.Forms.Form
    {
        public ExampleForm()
        {
            InitializeComponent();

            MyFormAppSession session = new MyFormAppSession("http://192.168.0.51/tc");
            HomeFolder home = new HomeFolder(this);
            Query query = new Query(this);
            DataManagement dm = new DataManagement(this);

            User user = session.login();

            DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());
            String[] attributes = { "os_username" };

            dmService.GetProperties(new ModelObject[] { user }, attributes);

            appendTxt("User name: " + user.Os_username);

            home.listHomeFolder(user);
            query.queryItems();
            dm.createReviseAndDelete();

            session.logout();
        }


        public void appendTxt(String txt)
        {
            userNameText.AppendText(txt + '\n');
        }

        public void printObjects(ModelObject[] objects)
        {
            if (objects == null)
                return;

            // Ensure that the referenced User objects that we will use below are loaded
            getUsers(objects);

            appendTxt("Name\t\tOwner\t\tLast Modified");
            appendTxt("====\t\t=====\t\t=============");
           
            for (int i = 0; i < objects.Length; i++)
            {
                if (!(objects[i] is WorkspaceObject))
                    continue;

                WorkspaceObject wo = (WorkspaceObject) objects[i];
                try
                {
                    String name = wo.Object_string;
                    User owner = (User) wo.Owning_user;
                    DateTime lastModified = wo.Last_mod_date;

                    appendTxt(name + "\t" + owner.User_name + "\t" + lastModified.ToString());
                }
                catch (NotLoadedException)
                {
                    MessageBox.Show("The Object Property Policy ($TC_DATA/soa/policies/Default.xml) is not configured with this property.");
                }
            }

        }

        private void getUsers(ModelObject[] objects)
        {
            if (objects == null)
                return;

            DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());
            ArrayList unKnownUsers = new ArrayList();
       
                for (int i = 0; i < objects.Length; i++)
            {
                if (!(objects[i] is WorkspaceObject ))
                    continue;

                WorkspaceObject wo = (WorkspaceObject) objects[i];

                User owner = null;
                try
                {
                    owner = (User) wo.Owning_user;
                    String userName = owner.User_name;
                }
                catch (NotLoadedException /*e*/)
                {
                    if (owner != null)
                        unKnownUsers.Add(owner);
                }
            }

            User[] users =  new User[unKnownUsers.Count];
            unKnownUsers.CopyTo(users);
            String[] attributes = { "user_name" };

            dmService.GetProperties(users, attributes);
        }
    }
}
