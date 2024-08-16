using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileMonitorService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _downloadsFolder;
        private FileSystemWatcher _fileSystemWatcher;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            _fileSystemWatcher = new FileSystemWatcher(_downloadsFolder);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _fileSystemWatcher.Created += OnFileCreated;
            _fileSystemWatcher.EnableRaisingEvents = true;

            _logger.LogInformation("File monitoring service started.");
            return Task.CompletedTask;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            if (e.Name.Contains("backend_entry_point_file"))
            {
                LogToEventViewer($"File detected: {e.FullPath}");
            }
        }

        private void LogToEventViewer(string message)
        {
            if (!EventLog.SourceExists("FileMonitorService"))
            {
                EventLog.CreateEventSource("FileMonitorService", "Application");
            }

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "FileMonitorService";
                eventLog.WriteEntry(message, EventLogEntryType.Information);
            }
        }

        public override void Dispose()
        {
            _fileSystemWatcher?.Dispose();
            base.Dispose();
        }
    }
}
