// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Api.Model;
using DeviceManager.Common;
using DeviceManager.Service;
using DeviceManager.Service.Model;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DeviceManager.Api.Controllers
{
    public class UserController : ApiController
    {
        private readonly IClientService _userService;
        private readonly ILogService _logService;

        public UserController(IClientService userService, ILogService<UserController> logService)
        {
            _userService = userService;
            _logService = logService;
        }

        // POST api/user/register
        [HttpPost]
        public async Task<HttpResponseMessage> Register([FromBody]string domainUserName)
        {
            if (string.IsNullOrEmpty(domainUserName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Null or empty parameter.");
            }

            HttpStatusCode statusCode = HttpStatusCode.OK;
            RegisterResult result = await _userService.RegisterClientAsync(domainUserName);
            if (result.Result == RegisterUserResult.AlreadyExists)
            {
                statusCode = HttpStatusCode.OK;
            }
            else if (result.Result == RegisterUserResult.Created)
            {
                statusCode = HttpStatusCode.Created;
            }
            return Request.CreateResponse(statusCode, result.User);
        }

        // POST api/user/setfriendlyname
        [HttpPost]
        public async Task<HttpResponseMessage> SetFriendlyName(SetFriendlyNameRequest request)
        {
            if (string.IsNullOrEmpty(request.DomainUserName) || string.IsNullOrEmpty(request.FriendlyName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Null or empty parameter.");
            }

            bool result = await _userService.SetFriendlyNameAsync(request.DomainUserName, request.FriendlyName);
            if (result)
            {
                _logService.LogInformation($"Set friendly name {request.FriendlyName} for {request.DomainUserName}");
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}