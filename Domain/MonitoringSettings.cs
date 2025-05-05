namespace SystemMonitorApp.Domain;

public class MonitoringSettings
{
    public int IntervalSeconds { get; set; }
    public string ApiEndpoint { get; set; } = string.Empty;
    public string LogFilePath { get; set; } = "log.txt";
}
