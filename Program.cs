using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SystemMonitorApp.Domain;
using SystemMonitorApp.Infrastructure;
using SystemMonitorApp.Plugins;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SystemMonitorApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var services = new ServiceCollection();
                services.Configure<MonitoringSettings>(options => configuration.GetSection("Monitoring").Bind(options));
                services.AddSingleton<ISystemMonitorService, SystemMonitorService>();
                services.AddSingleton<IMonitorPlugin, FileLoggerPlugin>();
                services.AddSingleton<IMonitorPlugin, ApiPlugin>();
                services.AddSingleton<IConfiguration>(configuration);

                var provider = services.BuildServiceProvider();

                var monitor = provider.GetRequiredService<ISystemMonitorService>();
                var plugins = provider.GetServices<IMonitorPlugin>();
                var settings = provider.GetRequiredService<IOptions<MonitoringSettings>>().Value;
                var interval = settings.IntervalSeconds;

                Console.WriteLine("Starting system monitor...");

                while (true)
                {
                    try
                    {
                        var data = await monitor.GetSystemUsageAsync();
                        Console.WriteLine(
                            $"CPU: {data.CpuUsage:F2}% | " +
                            $"RAM: {data.RamUsedMB:F2}/{data.TotalRamMB:F2} MB | " +
                            $"Disk: {data.DiskUsedMB:F2}/{data.TotalDiskMB:F2} MB");

                        foreach (var plugin in plugins)
                        {
                            try
                            {
                                await plugin.HandleAsync(data);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Plugin Error] {plugin.GetType().Name}: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Monitoring Error] {ex.Message}");
                    }

                    await Task.Delay(interval * 500);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"[Startup Error] Missing configuration: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Fatal Error] {ex}");
            }
        }
    }
}
