using DeviceManager.Api.Model;
using DeviceManager.Service;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
