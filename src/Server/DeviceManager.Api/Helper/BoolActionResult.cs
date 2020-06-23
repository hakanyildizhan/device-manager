// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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