// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using Flurl;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Update
{
    public class UpdateChecker : IUpdateChecker
    {
        private string _requestId;

        private readonly ILogService _logService;
        private readonly IManifestParser _manifestParser;
        private readonly string _baseURL;
        private readonly ITokenStore _tokenStore;

        public EventHandler<int> DownloadProgressChanged { get; set; }

        public UpdateChecker(
            ILogService<UpdateChecker> logService, 
            IManifestParser manifestParser,
            ITokenStore tokenStore)
        {
            _logService = logService;
            _manifestParser = manifestParser;
            _tokenStore = tokenStore;
            _baseURL = @"https://siemens.sharepoint.com/teams/DeviceManager/_api/web";
        }

        public async Task<UpdateCheckResult> CheckForUpdate(ServerUpdateRequest request)
        {
            Stream manifestStream = await GetManifest();

            if (manifestStream == null)
            {
                _logService.LogError("Cannot check for update, no manifest to read");
                return new UpdateCheckResult() { Success = false };
            }

            UpdatePackage package = _manifestParser.GetUpdateInfoForServer(manifestStream);

            if (package == null)
            {
                _logService.LogError("Cannot check for update, could not parse manifest");
                return new UpdateCheckResult() { Success = false };
            }

            bool updateAvailable = request.ServerVersion.Compare(package.Version) == VersionCompareResult.Older;
            _logService.LogInformation($"Update check complete, {(updateAvailable ? "" : "no")} update available");
            return new UpdateCheckResult() { Package = package, UpdateIsAvailable = updateAvailable };
        }

        public async Task<UpdateCheckResult> CheckForUpdate(ClientUpdateRequest request)
        {
            Stream manifestStream = await GetManifest();

            if (manifestStream == null)
            {
                _logService.LogError("Cannot check for update, no manifest to read");
                return new UpdateCheckResult() { Success = false };
            }

            UpdatePackage package = _manifestParser.GetUpdateInfoForClient(manifestStream, request.ServerVersion);

            if (package == null)
            {
                _logService.LogError("Cannot check for update, could not parse manifest");
                return new UpdateCheckResult() { Success = false };
            }

            bool updateAvailable = request.ClientVersion.Compare(package.Version) == VersionCompareResult.Older;
            _logService.LogInformation($"Update check complete, {(updateAvailable ? "" : "no")} update available");
            return new UpdateCheckResult() { Package = package, UpdateIsAvailable = updateAvailable };
        }

        public async Task<UpdateDownloadResult> DownloadUpdate(UpdateDownloadRequest request)
        {
            _requestId = request.RequestId;
            _logService.LogInformation("Starting update download");

            if (request == null || 
                request.Package == null || 
                string.IsNullOrEmpty(request.TargetDirectory) ||
                !Directory.Exists(request.TargetDirectory))
            {
                _logService.LogError("Missing download information, aborting operation");
            }

            string url = GenerateFileURLFromRelativeURL(request.Package.Url);
            string fileName = request.Package.Url.Contains('/') ? request.Package.Url.Split('/').Last() : request.Package.Url;
            string targetFilePath = Path.Combine(request.TargetDirectory, fileName);

            try
            {
                AccessToken token = await GetAccessToken();

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Authorization", "Bearer " + token.access_token);
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileAsync(new Uri(url), targetFilePath);
                }

                if (!File.Exists(targetFilePath) || new FileInfo(targetFilePath).Length == 0)
                {
                    _logService.LogError("Error downloading the update");
                    return new UpdateDownloadResult() { Success = false };
                }

                _logService.LogInformation("Downloaded update successfully to " + targetFilePath);
                return new UpdateDownloadResult() { Success = true, File = targetFilePath };
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Error downloading the update");
                return new UpdateDownloadResult() { Success = false };
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(_requestId, e.ProgressPercentage);
        }

        public async Task<AccessToken> GetAccessToken()
        {
            AccessToken token = _tokenStore.GetAccessToken();

            if (token != null && token.IsValid)
            {
                _logService.LogInformation("Getting access token from store");
                return token;
            }

            _logService.LogInformation("Getting access token");
            string url = @"https://accounts.accesscontrol.windows.net/38ae3bcd-9579-4fd4-adda-b42e1495d55a/tokens/OAuth/2";
            try
            {
                using (WebClient wc = new WebClient())
                {
                    var body = new System.Collections.Specialized.NameValueCollection();
                    body.Add("grant_type", "client_credentials");
                    body.Add("client_id", "9f231cc2-8573-4bc4-8f9b-1667637132c6@38ae3bcd-9579-4fd4-adda-b42e1495d55a");
                    body.Add("client_secret", "MZrTvIHaY15/q/qPA3RlTdo+X9ialrnen7Dlt0W224Q=");
                    body.Add("resource", "00000003-0000-0ff1-ce00-000000000000/siemens.sharepoint.com@38ae3bcd-9579-4fd4-adda-b42e1495d55a");

                    byte[] responsebytes = wc.UploadValues(url, "POST", body);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    token = JsonConvert.DeserializeObject<AccessToken>(responsebody);
                    await _tokenStore.StoreAccessToken(token);
                    _logService.LogInformation("Got access token successfully");
                    return token;
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Error occurred while getting access token");
                return null;
            }
        }

        public async Task<Stream> GetManifest()
        {
            AccessToken token = await GetAccessToken();
            _logService.LogInformation("Getting manifest");
            string url = GenerateFileURLFromRelativeURL("manifest.xml");

            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("Authorization", "Bearer " + token.access_token);
                    return wc.OpenRead(url);
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Error occurred while getting manifest");
                return null;
            }
        }

        private string GenerateFileURLFromRelativeURL(string relativeFileUrl)
        {
            string relativePath = Url.Combine(@"/teams/DeviceManager/Releases", relativeFileUrl);
            return Url.Combine(_baseURL, $"GetFileByServerRelativeUrl('{relativePath}')", "$value");
        }
    }
}
