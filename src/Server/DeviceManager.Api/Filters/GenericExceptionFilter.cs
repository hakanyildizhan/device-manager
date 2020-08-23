// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using System.Net;
using System.Net.Http;
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