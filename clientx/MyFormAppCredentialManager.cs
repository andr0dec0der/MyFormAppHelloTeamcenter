using System;
using System.IO;

using Teamcenter.Schemas.Soa._2006_03.Exceptions;
using Teamcenter.Soa;
using Teamcenter.Soa.Common;
using Teamcenter.Soa.Client;
using Teamcenter.Soa.Exceptions;
using HelloTeamcenter.form;

namespace Teamcenter.ClientX
{
    public class MyFormAppCredentialManager : CredentialManager
    {

        private String name          = null;
        private String password      = null;
        private String group         = "";          // default group
        private String role          = "";          // default role
        private String discriminator = "MyFormApp";   // always connect same user to same instance of server


        public int CredentialType
        {
            get { return SoaConstants.CLIENT_CREDENTIAL_TYPE_STD; }
        }

        public string[] GetCredentials(InvalidCredentialsException e)
        //throws CanceledOperationException
        {
            Console.WriteLine(e.Message);
            return PromptForCredentials();
        }

        public String[] GetCredentials(InvalidUserException e) 
        //throws CanceledOperationException
        {
            // Have not logged in yet, shoult not happen but just in case
            if (name == null) return PromptForCredentials();

            // Return cached credentials
            String[] tokens = { name, password, group, role, discriminator };
            return tokens;
        }

        public void SetGroupRole(String group, String role)
        {
            this.group = group;
            this.role = role;
        }

        public void SetUserPassword(String user, String password, String discriminator)
        {
            this.name = user;
            this.password = password;
            this.discriminator = discriminator;
        }

    
        public String[] PromptForCredentials() 
        //throws CanceledOperationException
        {
            try
            {
                LoginForm lForm = new LoginForm();
                lForm.ShowDialog();

                name = lForm.getUserName();

                if (name.Length == 0) 
                    throw new CanceledOperationException("User Name empty");

                password = lForm.getPassword();

                lForm.Close();
            }
            catch (IOException e)
            {
                throw new CanceledOperationException("Failed to get the name and password.\n" + e.Message);
            }

            return new String[] { name, password, group, role, discriminator };
        }

    }
}
