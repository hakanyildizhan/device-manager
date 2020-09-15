// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using NCrontab;
using System;

namespace DeviceManager.Service
{
    /// <summary>
    /// Utility that parses crontab statements.
    /// </summary>
    public class CrontabParser : IScheduleParser
    {
        public ScheduleValidationResult Check(string expression)
        {
            try
            {
                CrontabSchedule.TryParse(expression);
                return new ScheduleValidationResult { IsValid = true };
            }
            catch (Exception ex)
            {
                return new ScheduleValidationResult { IsValid = false, ValidationError = ex.Message };
            }
        }

        public DateTime? GetNextOccurence(string expression)
        {
            try
            {
                return CrontabSchedule.Parse(expression).GetNextOccurrence(DateTime.UtcNow);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
