// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service.Model;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public static class DeviceListSelect
    {
        public static IEnumerable<DeviceListViewModel> MapDeviceToViewModel(this IEnumerable<Device> devices, string headerFormat)
        {
            var headers = GetHeaders(headerFormat);
            devices.FormatPrecedingColumns(headers.Count);
            var widths = GetHeaderWidthsByGroup(devices, headers);

            var deviceListVM = devices.GroupBy(d => d.DeviceGroup).Select(d => new DeviceListViewModel
            {
                Type = d.Key,
                DeviceList = new ObservableCollection<DeviceItemViewModel>(
                    d.AsEnumerable()
                    .OrderBy(i => typeof(Device).GetProperty(headers[0]).GetValue(i))
                    .Select(i => new DeviceItemViewModel
                {
                    Id = i.Id,
                    DeviceName = i.Name,
                    Header1 = typeof(Device).GetProperty(headers[0]).GetValue(i).ToString(),
                    Header2 = headers.Count >= 2 ? typeof(Device).GetProperty(headers[1]).GetValue(i).ToString() : string.Empty,
                    Header3 = headers.Count == 3 ? typeof(Device).GetProperty(headers[2]).GetValue(i).ToString() : string.Empty,
                    Header1Width = widths[d.Key][0],
                    Header2Width = widths[d.Key][1],
                    Header3Width = widths[d.Key][2],
                    ConnectedModuleInfo = i.ConnectedModuleInfo,
                    IsAvailable = i.IsAvailable,
                    UsedBy = i.UsedBy,
                    UsedByFriendly = i.UsedByFriendly,
                    CheckoutDate = i.CheckoutDate
                }))
            });

            return deviceListVM;
        }

        /// <summary>
        /// Gets widths of requested headers for the menu items for each device group.
        /// </summary>
        /// <param name="deviceList"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Dictionary<string, List<int>> GetHeaderWidthsByGroup(IEnumerable<Device> deviceList, List<string> headers)
        {
            var maxHeaderWidthsByGroup = new Dictionary<string, List<int>>();
            
            deviceList.GroupBy(d => d.DeviceGroup).ToList().ForEach(devices =>
            {
                var maxWidths = new List<int>();

                var header1Widths = devices.Select(a => typeof(Device).GetProperty(headers[0]).GetValue(a).ToString().Length).OrderByDescending(a => a);
                int header1Width = header1Widths.First() * 6 + (header1Widths.First() - header1Widths.Last() <= 5 ? 20 : 0);
                maxWidths.Add(header1Width);

                var header2Widths = headers.Count >= 2 ? devices.Select(a => typeof(Device).GetProperty(headers[1]).GetValue(a).ToString().Length).OrderByDescending(a => a) : null;
                int header2Width = header2Widths == null ? 0 : header2Widths.First() * 6 + (header2Widths.First() - header2Widths.Last() <= 5 ? 20 : 0);

                if (headers.Count == 2)
                {
                    header2Width -= 30;
                }

                maxWidths.Add(header2Width);

                var header3Widths = headers.Count == 3 ? devices.Select(a => typeof(Device).GetProperty(headers[2]).GetValue(a).ToString().Length).OrderByDescending(a => a) : null;
                int header3Width = header3Widths == null ? 0 : header3Widths.First() * 6 + (header3Widths.First() - header3Widths.Last() <= 5 ? 20 : 0);
                header3Width -= 30;
                maxWidths.Add(header3Width);

                maxHeaderWidthsByGroup.Add(devices.Key, maxWidths);
            });

            return maxHeaderWidthsByGroup;
        }

        /// <summary>
        /// Gets headers contained in the given string in comma separated format. In case these values do not correspond to valid header names, default headers are returned.
        /// </summary>
        /// <param name="headerFormat"></param>
        /// <returns></returns>
        static List<string> GetHeaders(string headerFormat)
        {
            var defaultHeaders = new List<string>() { "Name", "HardwareInfo", "Address" };

            if (string.IsNullOrEmpty(headerFormat))
            {
                return defaultHeaders;
            }

            string[] headers = headerFormat.Contains(',') ? headerFormat.Split(',') : new string[] { headerFormat };

            var properties = new List<string>();

            for (int i = 0; i < headers.Length; i++)
            {
                var property = typeof(Device).GetProperty(headers[i]);
                if (property != null)
                {
                    properties.Add(property.Name);
                }
            }

            return properties.Count == 0 ? defaultHeaders : properties;
        }

        /// <summary>
        /// Standardizes widths of columns except the last one by filling them with space character. In case there is only one header, no formatting is done.
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="headerCount"></param>
        /// <returns></returns>
        static IList<Device> FormatPrecedingColumns(this IEnumerable<Device> devices, int headerCount)
        {
            if (headerCount == 1)
            {
                return devices.ToList();
            }

            var devicesGroupedByType = devices.GroupBy(d => d.DeviceGroup);
            IList<Device> formattedDeviceList = new List<Device>();
            
            foreach (var deviceList in devicesGroupedByType)
            {
                int maxDeviceNameLength = deviceList.Select(i => i.Name.Length).OrderByDescending(n => n).FirstOrDefault();
                deviceList.ToList().ForEach(d => d.Name = AddTrailingSpacesToString(d.Name, maxDeviceNameLength - d.Name.Length));

                if (headerCount == 3)
                {
                    int maxHWInfoLength = deviceList.Select(i => i.HardwareInfo.Length).OrderByDescending(i => i).FirstOrDefault();
                    deviceList.ToList().ForEach(d => d.HardwareInfo = AddTrailingSpacesToString(d.HardwareInfo, maxHWInfoLength - d.HardwareInfo.Length));
                }

                deviceList.ToList().ForEach(d => formattedDeviceList.Add(d));
            }

            return formattedDeviceList;
        }

        /// <summary>
        /// Adds spaces in the given amount to the input text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="spaces"></param>
        /// <returns></returns>
        static string AddTrailingSpacesToString(string text, int spaces)
        {
            return new StringBuilder().Append(text).Append(new string('\xA0', spaces)).ToString();
        }
    }
}