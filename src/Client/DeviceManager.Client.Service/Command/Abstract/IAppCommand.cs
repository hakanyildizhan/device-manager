// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    /// <summary>
    /// A command to be invoked by the <see cref="CommandFactory"/>. Each command implementation must derive from this interface and also be registered in the CommandRegistry XML.
    /// </summary>
    public interface IAppCommand
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Parameters required to execute this command.
        /// </summary>
        Dictionary<string,string> Parameters { get; set; }

        /// <summary>
        /// Executes the command and returns a success/failure state.
        /// </summary>
        /// <returns></returns>
        Task<bool> Execute();
    }
}
