// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public interface IFeedbackService
    {
        Task ShowMessageAsync(MessageType messageType, string title, string message);
        Task ShowMessageAsync(string title, string message);
        Task ShowMessageAsync(MessageType messageType, string message);
        Task ShowMessageAsync(string message);
    }
}
