using System;

using Teamcenter.Soa.Client;
using System.Windows.Forms;

namespace Teamcenter.ClientX
{
    public class MyFormAppRequestListener : RequestListener
    {
        public void ServiceRequest(ServiceInfo info)
        {
            // will log the service name when done
        }

        public void ServiceResponse(ServiceInfo info)
        {
            //MessageBox.Show(info.Id + ": " + info.Service + "." + info.Operation);
        }
    }
}
