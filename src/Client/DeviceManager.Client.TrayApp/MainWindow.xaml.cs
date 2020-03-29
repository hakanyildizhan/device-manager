using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeviceManager.Client.TrayApp.ViewModel;

namespace DeviceManager.Client.TrayApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //AddDummyData();
            this.DataContext = new MainWindowViewModel();
        }

        private void AddDummyData()
        {
            DeviceItemControl device1 = new DeviceItemControl()
            {
                DataContext = new DeviceItemViewModel
                {
                    Id = 1,
                    Name = "3RW55 V2.1 [192.168.100.50]",
                    Tooltip = "MLFB: 3RW5 513-*HA**\nPlugged modules: PN HF\nAccessible via: IM",
                    IsAvailable = true
                }
            };

            DeviceItemControl device2 = new DeviceItemControl()
            {
                DataContext = new DeviceItemViewModel
                {
                    Id = 2,
                    Name = "3RW55-F V1.0 [192.168.100.52]",
                    Tooltip = "MLFB: 3RW5 548-*HF**\nPlugged modules: PN ST, HMI HF [V3.0]\nAccessible via: IM + HMI",
                    IsAvailable = false
                    //UsedBy = "Sefa"
                }
            };

            DeviceItemControl device3 = new DeviceItemControl()
            {
                DataContext = new DeviceItemViewModel
                {
                    Id = 3,
                    Name = "3RW52 T2.0 [14]",
                    Tooltip = "MLFB: 3RW5 513-*HA**\nPlugged modules: DP, HMI HF [T3.0]\nAccessible via: IM",
                    IsAvailable = true
                }
            };

            trayIcon.ContextMenu.Items.Add(device1);
            trayIcon.ContextMenu.Items.Add(device2);
            trayIcon.ContextMenu.Items.Add(device3);
        }
    }
}
