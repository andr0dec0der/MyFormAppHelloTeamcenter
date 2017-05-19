using System;
using System.Collections;
using System.Net;

using Teamcenter.Schemas.Soa._2006_03.Exceptions;
using Teamcenter.Services.Strong.Core;
using Teamcenter.Services.Strong.Core._2006_03.Session;
using Teamcenter.Soa;
using Teamcenter.Soa.Client;
using Teamcenter.Soa.Client.Model;
using Teamcenter.Soa.Exceptions;

using WorkspaceObject = Teamcenter.Soa.Client.Model.Strong.WorkspaceObject;
using User            = Teamcenter.Soa.Client.Model.Strong.User;
using System.Windows.Forms;

namespace Teamcenter.ClientX
{
    public class MyFormAppSession
    {
        private static Connection connection;
        private static MyFormAppCredentialManager credentialManager;

        public MyFormAppSession(String host)
        {
            credentialManager = new MyFormAppCredentialManager();
            string proto = null;
            string envNameTccs = null;

            if (host.StartsWith("http"))
            {
                proto = SoaConstants.HTTP;
            }
            else if (host.StartsWith("tccs"))
            {
                proto = SoaConstants.TCCS;
                int envNameStart = host.IndexOf('/') + 2;
                envNameTccs = host.Substring(envNameStart, host.Length - envNameStart);

            }

            connection = new Connection(host, new System.Net.CookieCollection(), credentialManager, SoaConstants.REST, proto, false);

            if (proto == SoaConstants.TCCS)
                connection.SetOption(Connection.TCCS_ENV_NAME, envNameTccs);

            connection.ExceptionHandler = new MyFormAppExceptionHandler();
            connection.ModelManager.AddPartialErrorListener(new MyFormAppPartialErrorListener());
            connection.ModelManager.AddModelEventListener(new MyFormAppModelEventListener());

            Connection.AddRequestListener(new MyFormAppRequestListener());
        }

        public static Connection getConnection()
        {
            return connection;
        }

        public User login()
        {
            SessionService sessionService = SessionService.getService(connection);

            try
            {
                String[] credentials = credentialManager.PromptForCredentials();
                while (true)
                {
                    try
                    {
                        LoginResponse resp = sessionService.Login(credentials[0], credentials[1], credentials[2], credentials[3], "", credentials[4]);

                        return resp.User;
                    }
                    catch (InvalidCredentialsException e)
                    {
                        credentials = credentialManager.GetCredentials(e);
                    }
                }
            } catch (CanceledOperationException) { }

            return null;
        }

        public void logout()
        {
            SessionService sessionService = SessionService.getService(connection);
           
            try
            {
                sessionService.Logout();
            }
            catch (ServiceException) { }
        }
    }
}
