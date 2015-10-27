using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace eToro.Utils.Timers
{
    public class TimerWrapper : ITimerWrapper
    {
        private readonly Timer _timer = new Timer();

        public void Start()
        {
            _timer.Enabled = true;
        }

        public void Stop()
        {
            _timer.Enabled = false;
        }

        public void Initialize(double interval)
        {
            _timer.Interval = interval;
            _timer.Enabled = false;
            _timer.Elapsed += (sender, args) =>
            {
                if (Elapsed != null)
                {
                    Elapsed(this, args);
                }
            };
        }

        public event ElapsedEventHandler Elapsed;

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
