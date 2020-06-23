// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace DeviceManager.Client.TrayApp
{
    /// <summary>
    /// Attached property for a <see cref="Hyperlink"/> in order to make its <see cref="Hyperlink.NavigateUri"/> property automatically navigatable.
    /// </summary>
    public class HyperlinkNavigateProperty : BaseAttachedProperty<HyperlinkNavigateProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            base.OnValueChanged(sender, e);
            Hyperlink hyperlink;

            try
            {
                hyperlink = sender as Hyperlink;
            }
            catch (Exception)
            {
                return; // not a Hyperlink
            }

            if ((bool)e.NewValue)
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            else
                hyperlink.RequestNavigate -= Hyperlink_RequestNavigate;
        }

        private static void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
