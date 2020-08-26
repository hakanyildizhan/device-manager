// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DeviceManager.Client.TrayApp.Service
{
    /// <summary>
    /// A singleton event aggregator to coordinate prompt subscribers and publishers.
    /// </summary>
    public class EventAggregator
    {
        private static readonly Dictionary<object, List<Delegate>> m_events = new Dictionary<object, List<Delegate>>();
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
            Monitor.Enter(m_events);
            List<Delegate> d;
            bool delegatesForObjectExists = m_events.TryGetValue(eventKey, out d);

            if (!delegatesForObjectExists)
            {
                m_events.Add(eventKey, new List<Delegate>() { handler });
            }
            else
            {
                d.Add(handler);
                m_events[eventKey] = d;
            }

            Monitor.Exit(m_events);
        }

        public void Remove(object eventKey, Delegate handler)
        {
            Monitor.Enter(m_events);
            List<Delegate> delegateList;

            if (m_events.TryGetValue(eventKey, out delegateList))
            {
                Delegate d = delegateList.Where(dlg => dlg == handler).FirstOrDefault();
                delegateList.Remove(d);
                d = Delegate.Remove(d, handler);

                if (!delegateList.Any())
                {
                    m_events.Remove(eventKey);
                }
                else
                {
                    m_events[eventKey] = delegateList;
                }
            }

            Monitor.Exit(m_events);
        }

        public void Raise(object eventKey, object sender, object e)
        {
            List<Delegate> delegateList;
            Monitor.Enter(m_events);
            m_events.TryGetValue(eventKey, out delegateList);
            Monitor.Exit(m_events);

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
                d.DynamicInvoke(new Object[] { sender, e });
                Remove(eventKey, d);
            }
        }
    }
}
