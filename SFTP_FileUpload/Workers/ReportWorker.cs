using SFTP_FileUpload.BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFTP_FileUpload.Workers
{
    public class ReportDataWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IReportService _reportsService;
        private readonly int _serviceDelay;

        public ReportDataWorker(IConfiguration configuration, IReportService reportService)
        {
            _configuration = configuration;
            _reportsService = reportService;

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string MainGuid = Guid.NewGuid().ToString();
                //WorkerStatusConfig.SmsWorkerStatus = WorkerStatus.Running;
                await _reportsService.UplaodFile(MainGuid);
                // WorkerStatusConfig.SmsWorkerStatus = WorkerStatus.Delayed;
                await Task.Delay(_serviceDelay, stoppingToken);
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken = default)
        {
             await base.StopAsync(stoppingToken);
        }
        public override async Task StartAsync(CancellationToken stoppingToken = default)
        {        
            await base.StartAsync(stoppingToken);

        }
    }
}
