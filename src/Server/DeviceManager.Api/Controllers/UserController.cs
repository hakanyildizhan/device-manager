using DeviceManager.Api.Model;
using DeviceManager.Service;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DeviceManager.Api.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/user/register
        [HttpPost]
        public async Task<HttpResponseMessage> Register([FromBody]string domainUserName)
        {
            if (string.IsNullOrEmpty(domainUserName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Null or empty parameter.");
            }
            try
            {
                HttpStatusCode statusCode = HttpStatusCode.OK;
                RegisterResult result = await _userService.RegisterUserAsync(domainUserName);
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
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error");
            }
        }

        // POST api/user/setfriendlyname
        [HttpPost]
        public async Task<HttpResponseMessage> SetFriendlyName(SetFriendlyNameRequest request)
        {
            if (string.IsNullOrEmpty(request.DomainUserName) || string.IsNullOrEmpty(request.FriendlyName))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Null or empty parameter.");
            }
            try
            {
                bool result = await _userService.SetFriendlyNameAsync(request.DomainUserName, request.FriendlyName);
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error");
            }
        }
    }
}