// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System;

namespace DeviceManager.Service
{
    public interface IScheduleParser
    {
        /// <summary>
        /// Checks if the schedule statement is valid.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        ScheduleValidationResult Check(string expression);

        /// <summary>
        /// Gets the date and time that the next occurence of given schedule statement points to.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        DateTime? GetNextOccurence(string expression);
    }
}
