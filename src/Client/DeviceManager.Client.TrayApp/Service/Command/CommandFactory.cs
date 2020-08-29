// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.IoC;
using DeviceManager.Client.TrayApp.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;

namespace DeviceManager.Client.TrayApp.Command
{
    /// <summary>
    /// Singleton factory that processes commands sent in the HTTP Query string like format.
    /// For the command to be found, it has to be registered in the CommandRegistry XML and must derive from <see cref="IAppCommand"/>.
    /// </summary>
    public class CommandFactory
    {
        private readonly Dictionary<string, IAppCommand> _commandList;
        private static CommandFactory _commandFactoryInstance;
        private static object _lockObject = new object();

        private CommandFactory()
        {
            _commandList = new Dictionary<string, IAppCommand>();
            Initialize();
        }

        public static CommandFactory Instance
        {
            get
            {
                if (_commandFactoryInstance == null)
                {
                    lock (_lockObject)
                    {
                        _commandFactoryInstance = new CommandFactory();
                    }
                }
                return _commandFactoryInstance;
            }
        }

        /// <summary>
        /// Instantiates the command contained in the given query (if it exists), sets given query parameters on the instantiated command (if any) then returns the command. 
        /// </summary>
        /// <param name="commandQuery"></param>
        /// <returns></returns>
        public IAppCommand GetCommand(string commandQuery)
        {
            if (!commandQuery.Contains('?'))
            {
                return _commandList.ContainsKey(commandQuery) ? _commandList[commandQuery] : null;
            }

            string[] queryParts = commandQuery.Split('?');
            string commandName = queryParts[0];
            var parameters = HttpUtility.ParseQueryString(queryParts[1]);
            var commandParameters = new Dictionary<string, string>();

            foreach (var key in parameters)
            {
                commandParameters.Add(key.ToString(), parameters[key.ToString()]);
            }

            var command = _commandList.ContainsKey(commandName) ? _commandList[commandName] : null;

            if (command != null)
            {
                typeof(IAppCommand).GetProperty("Parameters").SetValue(command, commandParameters);
            }

            return command;
        }

        /// <summary>
        /// Instantiates all registered commands, together with their injected dependencies.
        /// </summary>
        private void Initialize()
        {
            XElement manifest = XElement.Parse(Resources.CommandRegistry);
            IList<XElement> metaList = manifest.Descendants("CommandMetaInfo").ToList();

            foreach (var commandMeta in metaList)
            {
                string commandId = commandMeta.Element("CommandId").Value;
                string commandAssembly = commandMeta.Element("Assembly").Value;
                string commandClassName = commandMeta.Element("ClassName").Value;

                var assembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, commandAssembly));
                var commandType = assembly.GetType(commandClassName);
                var constructors = commandType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                var injectedTypes = constructors[0].GetParameters().Select(p => p.ParameterType);
                IAppCommand command = null;
                
                if (!injectedTypes.Any())
                {
                    command = (IAppCommand)assembly.CreateInstance(commandClassName);
                }

                else
                {
                    var injectedTypeList = new List<object>();

                    foreach (var injectedType in injectedTypes)
                    {
                        injectedTypeList.Add(UnityServiceProvider.Instance.GetService(injectedType));
                    }

                    command = (IAppCommand)assembly.CreateInstance(commandClassName, false, BindingFlags.CreateInstance, null, injectedTypeList.ToArray(), null, null);
                    _commandList.Add(commandId, command);
                }
            }
        }
    }
}
