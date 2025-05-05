using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using SystemMonitorApp.Domain;

namespace SystemMonitorApp.Plugins;

public class FileLoggerPlugin : IMonitorPlugin
{
    private readonly string _filePath;
    private const string DefaultLogFile = "log.txt";  

    public FileLoggerPlugin(IConfiguration config)
    {
        _filePath = config.GetValue<string>("Monitoring:LogFilePath") ?? DefaultLogFile;

        if (string.IsNullOrWhiteSpace(_filePath))
        {
            throw new ArgumentException("Log file path cannot be empty");
        }
    }

    public async Task HandleAsync(SystemUsageData data)
    {
        var line = $"{DateTime.Now:u} | " +
                 $"CPU: {data.CpuUsage,6:F2}% | " +
                 $"RAM: {data.RamUsedMB,7:F2} / {data.TotalRamMB,7:F2} MB | " +
                 $"Disk: {data.DiskUsedMB,7:F2} / {data.TotalDiskMB,7:F2} MB";

        try
        {
            // Create directory if it doesn't exist
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.AppendAllTextAsync(_filePath, line + Environment.NewLine);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"[ERROR] Failed to write to {_filePath}: {ex.Message}");
           
        }
        catch (IOException ex) when (ex.Message.Contains("disk full", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"[CRITICAL] Disk full - cannot log: {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UNHANDLED] {ex.GetType().Name} in FileLoggerPlugin: {ex.Message}");
        }
    }
}