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
        [HttpPost]
        public HttpResponseMessage IsDeviceAvailable([FromBody]int deviceId)
        {
            bool available = _sessionService.IsDeviceAvailable(deviceId);
            return Request.CreateResponse(HttpStatusCode.OK, available);
        }

        // POST api/session/create
        [HttpPost]
        public async Task<HttpResponseMessage> Create(SessionRequest request)
        {

            bool sessionCreated = await _sessionService.CreateSessionAsync(request.UserName, request.DeviceId);
            return Request.CreateResponse(HttpStatusCode.Created, sessionCreated);
        }

        // PUT api/session/end
        [HttpPost]
        public async Task<HttpResponseMessage> End(SessionRequest request)
        {
            bool sessionEnded = await _sessionService.EndSessionAsync(request.UserName, request.DeviceId);
            return Request.CreateResponse(HttpStatusCode.OK, sessionEnded);
        }

        // PUT api/session/endall
        [HttpPost]
        public async Task<HttpResponseMessage> EndAll([FromBody]string domainUserName)
        {
            bool success = await _sessionService.EndAllSessionsOfUserAsync(domainUserName);
            return Request.CreateResponse(HttpStatusCode.OK, success);
        }
    }
}
