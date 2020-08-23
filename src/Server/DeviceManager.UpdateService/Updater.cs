using DeviceManager.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DeviceManager.UpdateService
{
    public partial class Updater : UpdateServiceBase
    {
        static Timer _timer = new Timer();
        private readonly IUpdateService UpdateService = (IUpdateService) ServiceProvider.GetService<IUpdateService>();

        public Updater()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }

        protected override void OnStop()
        {

        }
    }
}
