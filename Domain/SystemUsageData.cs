namespace SystemMonitorApp.Domain;

public class SystemUsageData
{
    public double CpuUsage { get; set; }
    public double RamUsedMB { get; set; }
    public double TotalRamMB { get; set; }
    public double DiskUsedMB { get; set; }
    public double TotalDiskMB { get; set; }
}
