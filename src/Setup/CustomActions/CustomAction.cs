using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;

namespace DeviceManager.Setup.CustomActions
{
    public class CustomActions
    {
        static HttpClient client = new HttpClient();

        [CustomAction]
        public static ActionResult TestServer(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            //session.Log("Begin CustomAction");

            string address = session["SERVERADDRESS"];

            // if address does not start with http or https, add it
            if (!address.StartsWith("http") && !address.StartsWith("https"))
            {
                address = "http://" + address;
            }

            // if address does not end with '/', add it
            if (!address.EndsWith("/"))
            {
                address = address + '/';
            }

            // send a request to <address>/api/user/register
            UriBuilder builder = new UriBuilder($"{address}api/");
            Uri uri = new Uri(builder.Uri, "user/register");
            string success = "0";
             
            try
            {
                string domainUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                HttpResponseMessage response = Task.Run(async () => await client.PostAsJsonAsync(uri, domainUserName)).Result;
                if (response.IsSuccessStatusCode)
                {
                    success = "1";
                }
            }
            catch (Exception)
            {
                
            }

            session["TESTRESULT"] = success;
            return ActionResult.Success;
        }
    }
}
