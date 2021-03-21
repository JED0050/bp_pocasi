using AgregaceDatLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Webova_Sluzba
{
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private AvgForecast forecastService;

        public TimedHostedService()
        {
            forecastService = new AvgForecast("");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                forecastService.SaveForecastBitmaps();
            }
            catch(Exception e)
            {
                Debug.WriteLine("Při vytváření bitmap došlo k chybě! " + e.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
