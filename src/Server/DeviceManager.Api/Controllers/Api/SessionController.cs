// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Api.Model;
using DeviceManager.Service;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DeviceManager.Api.Controllers
{
    public class SessionController : ApiController
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // POST api/session/isDeviceAvailable
        /// <summary>
        /// Returns true if a device item with given ID is currently not checked out to anybody.
        /// </summary>
        /// <param name="deviceId">ID of the device item to check.</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsDeviceAvailable([FromBody]int deviceId)
        {
            bool available = _sessionService.IsDeviceAvailable(deviceId);
            return Request.CreateResponse(HttpStatusCode.OK, available);
        }

        // POST api/session/create
        /// <summary>
        /// Checks out given device item to the specified user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Create(SessionRequest request)
        {

            bool sessionCreated = await _sessionService.CreateSessionAsync(request.UserName, request.DeviceId);
            return Request.CreateResponse(HttpStatusCode.Created, sessionCreated);
        }

        // POST api/session/end
        /// <summary>
        /// Checks in given device item currently checked out to the specified user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> End(SessionRequest request)
        {
            bool sessionEnded = await _sessionService.EndSessionAsync(request.UserName, request.DeviceId);
            return Request.CreateResponse(HttpStatusCode.OK, sessionEnded);
        }

        // POST api/session/endall
        /// <summary>
        /// Checks in all device items currently checked out to the specified user.
        /// </summary>
        /// <param name="domainUserName">Windows username of the user.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> EndAll([FromBody]string domainUserName)
        {
            bool success = await _sessionService.EndAllSessionsOfUserAsync(domainUserName);
            return Request.CreateResponse(HttpStatusCode.OK, success);
        }
    }
}
