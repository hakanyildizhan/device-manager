// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Api.Model
{
    public class SessionRequest
    {
        public string UserName { get; set; }
        public int DeviceId { get; set; }
    }
}