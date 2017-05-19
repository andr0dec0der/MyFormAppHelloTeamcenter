using System;

using Teamcenter.Soa.Client.Model;
using Teamcenter.Soa.Exceptions;
using System.Windows.Forms;

namespace Teamcenter.ClientX
{
    public class MyFormAppModelEventListener : ModelEventListener
    {

        override public void LocalObjectChange(ModelObject[] objects)
        {
            if (objects.Length == 0) return;
   
            MessageBox.Show("Modified Objects handled in com.teamcenter.clientx.AppXUpdateObjectListener.modelObjectChange\n" +
            "The following objects have been updated in the client data model:");
           
            for (int i = 0; i < objects.Length; i++)
            {
                String uid = objects[i].Uid;
                String type = objects[i].GetType().Name;
                String name = "";
              
                if (objects[i].GetType().Name.Equals("WorkspaceObject"))
                {
                    ModelObject wo = objects[i];
                  
                    try
                    {
                        name = wo.GetProperty("object_string").StringValue;
                    } catch (NotLoadedException) {}
                }
            }
        }

        override public void LocalObjectDelete(string[] uids)
        {
            if (uids.Length == 0)
                return;

            MessageBox.Show("Deleted Objects handled in com.teamcenter.clientx.AppXDeletedObjectListener.modelObjectDelete\n" +
            "The following objects have been deleted from the server and removed from the client data model:");
        }
    }
}
