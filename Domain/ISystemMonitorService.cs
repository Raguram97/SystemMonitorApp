using System.Threading.Tasks;

namespace SystemMonitorApp.Domain;

public interface ISystemMonitorService
{
    Task<SystemUsageData> GetSystemUsageAsync();
}
