using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DeviceManager.Api.Helper
{
    public class BoolActionResult : IHttpActionResult
    {
        bool _value;
        HttpRequestMessage _request;

        public BoolActionResult(bool value, HttpRequestMessage request)
        {
            _value = value;
            _request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(_value.ToString()),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}