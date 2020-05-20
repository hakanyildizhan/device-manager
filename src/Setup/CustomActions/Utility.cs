﻿using System;
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
    }
}