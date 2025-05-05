using System.Threading.Tasks;

namespace SystemMonitorApp.Domain;

public interface IMonitorPlugin
{
    Task HandleAsync(SystemUsageData data);
}
