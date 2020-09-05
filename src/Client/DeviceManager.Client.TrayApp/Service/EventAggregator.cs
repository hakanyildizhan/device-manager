// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DeviceManager.Client.TrayApp.Service
{
    /// <summary>
    /// A singleton event aggregator to coordinate prompt subscribers and publishers.
    /// </summary>
    public class EventAggregator
    {
        private static readonly ConcurrentDictionary<object, List<Delegate>> _events = new ConcurrentDictionary<object, List<Delegate>>();
        private static EventAggregator _eventAggregatorInstance;
        private static object _lockObject = new object();

        public static EventAggregator Instance
        {
            get
            {
                if (_eventAggregatorInstance == null)
                {
                    lock (_lockObject)
                    {
                        _eventAggregatorInstance = new EventAggregator();
                    }
                }
                return _eventAggregatorInstance;
            }
        }

        public void Add(object eventKey, Delegate handler)
        {
            _events.AddOrUpdate(eventKey, new List<Delegate> { handler }, (key, existingDlgList) =>
            {
                existingDlgList.Add(handler);
                return existingDlgList;
            });
        }

        public void Remove(object eventKey, Delegate handler)
        {
            List<Delegate> delegateList;
            bool keyExists = _events.TryRemove(eventKey, out delegateList);

            if (keyExists && delegateList != null && delegateList.Count > 1)
            {
                bool removed = delegateList.Remove(handler);

                if (removed)
                {
                    _events.TryAdd(eventKey, delegateList);
                }
            }
        }

        /// <summary>
        /// Raise the event registered by the subscriber. In case the subscriber has not registered any events, no event is raised.
        /// </summary>
        /// <param name="eventKey"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Raise(object eventKey, object sender, object e)
        {
            List<Delegate> delegateList;
            
            if (!_events.TryGetValue(eventKey, out delegateList))
            {
                return; // no subscriber found
            }

            Delegate d = null;

            foreach (var dlg in delegateList)
            {
                if (dlg.Method.GetParameters().Length > 1 && dlg.Method.GetParameters()[1].ParameterType == e.GetType())
                {
                    d = dlg;
                    break;
                }
            }

            if (d != null)
            {
                d.DynamicInvoke(new object[] { sender, e });
                Remove(eventKey, d);
            }
        }
    }
}
