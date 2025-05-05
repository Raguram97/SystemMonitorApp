using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SystemMonitorApp.Domain;

namespace SystemMonitorApp.Plugins;

public class ApiPlugin : IMonitorPlugin
{
    private readonly string _endpoint = ""; //paste the API
    private static readonly HttpClient _client = new();


    public ApiPlugin(IConfiguration config)
    {
        _endpoint = config["Monitoring:ApiEndpoint"] ?? _endpoint;
    }

    public async Task HandleAsync(SystemUsageData data)
    {
        if (string.IsNullOrWhiteSpace(_endpoint))
        {
            Console.WriteLine("[ApiPlugin] Endpoint not configured.");
            return;
        }

        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                cpu = data.CpuUsage,
                ram_used = data.RamUsedMB,
                total_ram = data.TotalRamMB,
                disk_used = data.DiskUsedMB,
                total_disk = data.TotalDiskMB
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[ApiPlugin] Failed to post data. Status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ApiPlugin] HTTP request failed: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[ApiPlugin] JSON serialization failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ApiPlugin] Unexpected error: {ex.Message}");
        }
    }
}
