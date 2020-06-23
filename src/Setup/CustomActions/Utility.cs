// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Microsoft.Win32;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceManager.Setup.CustomActions
{
    public static class Utility
    {
        static HttpClient _client = new HttpClient();

        public static ServerValidityTestResult TestServerValidity(string address)
        {
            ServerValidityTestResult result = new ServerValidityTestResult();

            try
            {
                // send a request to <address>/api/user/register
                UriBuilder builder = new UriBuilder($"{address}api/");
                Uri uri = new Uri(builder.Uri, "user/register");

                string domainUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                HttpResponseMessage response = Task.Run(async () => await _client.PostAsJsonAsync(uri, domainUserName)).Result;
                if (response.IsSuccessStatusCode)
                {
                    result.Message = "Server check was successful";
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Message = $"Error occured while checking server validity. Error type {ex.GetType().Name}, message: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Writes Device Manager server address to the registry.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool SaveServerURLToRegistry(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            RegistryKey regKey = Registry.CurrentUser;
            try
            {
                regKey = regKey.CreateSubKey("SOFTWARE\\Hakan Yildizhan\\DeviceManager", true);
                regKey.SetValue("ServerAddress", url, RegistryValueKind.String);
                regKey.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
