// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.TrayApp.IoC;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A global lock for property checks so prevent locking on different instances of expressions.
        /// Considering how fast this check will always be it isn't an issue to globally lock all callers.
        /// </summary>
        protected object mPropertyValueCheckLock = new object();

        internal static IAppServiceProvider ServiceProvider => UnityServiceProvider.Instance;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        protected async Task RunCommandAsync(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            // Lock to ensure single access to check
            lock (mPropertyValueCheckLock)
            {
                // Check if the flag property is true (meaning the function is already running)
                if (updatingFlag.GetPropertyValue())
                    return;

                // Set the property flag to true to indicate we are running
                updatingFlag.SetPropertyValue(true);
                OnPropertyChanged(((updatingFlag.Body as MemberExpression).Member as System.Reflection.PropertyInfo).Name);
            }

            try
            {
                // Run the passed in action
                await action();
            }
            finally
            {
                // Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
                OnPropertyChanged(((updatingFlag.Body as MemberExpression).Member as System.Reflection.PropertyInfo).Name);
            }
        }

        /// <summary>
        /// Runs a command with specified delay time, if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating no running function) then the action is run.
        /// Once the action is finished if it was run, then the flag is reset to false
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag defining if the command is already running</param>
        /// <param name="delaySeconds">Amount of time in seconds to apply a delay before running the command</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        protected async Task<bool> RunDelayedCommandAsync(Expression<Func<bool>> updatingFlag, Expression<Func<int>> delaySeconds, Func<Task<bool>> action)
        {
            // Lock to ensure single access to check
            lock (mPropertyValueCheckLock)
            {
                // Check if the flag property is true (meaning the function is already running)
                if (updatingFlag.GetPropertyValue())
                    return false;

                // Set the property flag to true to indicate we are running
                updatingFlag.SetPropertyValue(true);
                OnPropertyChanged(((updatingFlag.Body as MemberExpression).Member as System.Reflection.PropertyInfo).Name);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds.GetPropertyValue()));
                // Run the passed in action
                return await action();
            }
            finally
            {
                // Set the property flag back to false now it's finished
                updatingFlag.SetPropertyValue(false);
                OnPropertyChanged(((updatingFlag.Body as MemberExpression).Member as System.Reflection.PropertyInfo).Name);
            }
        }
    }
}
