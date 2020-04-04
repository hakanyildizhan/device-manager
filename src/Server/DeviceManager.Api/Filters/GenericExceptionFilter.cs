using DeviceManager.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace DeviceManager.Api
{
    public class GenericExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogService _logService;
        public GenericExceptionFilterAttribute()
        {
            _logService = (ILogService<GenericExceptionFilterAttribute>)System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ILogService<GenericExceptionFilterAttribute>));
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _logService.LogException(context.Exception);
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}