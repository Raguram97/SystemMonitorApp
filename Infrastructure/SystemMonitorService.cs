using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Management;
using SystemMonitorApp.Domain;
using System.Linq;

namespace SystemMonitorApp.Infrastructure
{
    public class SystemMonitorService : ISystemMonitorService
    {
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private readonly bool _isWindows;

        public SystemMonitorService()
        {
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (_isWindows)
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                _cpuCounter.NextValue();
            }
        }

        public async Task<SystemUsageData> GetSystemUsageAsync()
        {
            double cpu = 0;
            double ramUsed = 0;
            double diskUsed = 0;
            double totalRam = 0;
            double totalDisk = 0;

            try
            {
                if (_isWindows)
                {
                    try
                    {
                        _cpuCounter.NextValue();

                        await Task.Delay(1000);
                        cpu = _cpuCounter.NextValue();

                        totalRam = GetWindowsTotalMemory();
                        var availableMem = _ramCounter.NextValue();
                        ramUsed = totalRam - availableMem;

                        diskUsed = GetDiskUsedMB();
                        totalDisk = GetDiskTotalSizeMB();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Windows Monitoring Error] {ex.Message}");
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    try
                    {
                        cpu = await GetLinuxCpuUsageAsync();
                        ramUsed = GetLinuxUsedMemory();
                        totalRam = GetLinuxTotalMemory();
                        diskUsed = GetLinuxUsedDisk();
                        totalDisk = GetLinuxTotalDisk();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Linux Monitoring Error] {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SystemUsage Error] {ex.Message}");
            }

            return new SystemUsageData
            {
                CpuUsage = cpu,
                RamUsedMB = ramUsed,
                TotalRamMB = totalRam,
                DiskUsedMB = diskUsed,
                TotalDiskMB = totalDisk
            };
        }

        private double GetWindowsTotalMemory()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    double totalKb = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                    return totalKb / 1024.0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Memory WMI Error] {ex.Message}");
            }

            return 0;
        }

        private double GetDiskUsedMB()
        {
            try
            {
                var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady && d.DriveType == DriveType.Fixed);
                if (drive != null)
                {
                    var used = drive.TotalSize - drive.TotalFreeSpace;
                    return used / (1024.0 * 1024.0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Disk Error] {ex.Message}");
            }

            return 0;
        }

        private double GetDiskTotalSizeMB()
        {
            try
            {
                var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady && d.DriveType == DriveType.Fixed);
                if (drive != null)
                {
                    return drive.TotalSize / (1024.0 * 1024.0); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Disk Error] {ex.Message}");
            }

            return 0;
        }


        private static (ulong Idle, ulong Total) ReadLinuxCpuTimes()
        {
            var cpuLine = File.ReadLines("/proc/stat")
                              .FirstOrDefault(line => line.StartsWith("cpu "));

            if (string.IsNullOrWhiteSpace(cpuLine))
                return (0, 0);

            var parts = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                               .Skip(1)
                               .Select(s => ulong.TryParse(s, out var val) ? val : 0)
                               .ToArray();

            if (parts.Length < 5)
                return (0, 0);

            ulong idle = parts[3] + parts[4]; //idle+iowait time = idle time
            ulong total = parts.Aggregate(0UL, (sum, val) => sum + val);

            return (idle, total);
        }

        public async Task<float> GetLinuxCpuUsageAsync()
        {
            try
            {
                var (idle1, total1) = ReadLinuxCpuTimes();
                await Task.Delay(1500);
                var (idle2, total2) = ReadLinuxCpuTimes();

                var idleDelta = idle2 - idle1;
                var totalDelta = total2 - total1;

                if (totalDelta == 0) return 0;

                return 100f * (1f - ((float)idleDelta / totalDelta));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Linux CPU Error] {ex.Message}");
                return 0;
            }
        }


        private double GetLinuxUsedMemory()
        {
            try
            {
                var memInfo = File.ReadAllLines("/proc/meminfo");
                double total = 0, free = 0;

                foreach (var line in memInfo)
                {
                    if (line.StartsWith("MemTotal:"))
                        total = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]);
                    else if (line.StartsWith("MemAvailable:"))
                        free = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]);
                }

                return (total - free) / 1024.0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Linux Memory Error] {ex.Message}");
                return 0;
            }
        }

        private double GetLinuxTotalMemory()
        {
            try
            {
                var memInfo = File.ReadAllLines("/proc/meminfo");
                double total = 0;

                foreach (var line in memInfo)
                {
                    if (line.StartsWith("MemTotal:"))
                    {
                        total = double.Parse(line.Split(':')[1].Trim().Split(' ')[0]);
                    }
                }

                return total / 1024.0; // Convert to MB
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Linux Memory Error] {ex.Message}");
                return 0;
            }
        }

        private double GetLinuxUsedDisk()
        {
            try
            {
                var drive = new DriveInfo("/");
                if (drive.IsReady)
                {
                    var used = drive.TotalSize - drive.TotalFreeSpace;
                    return used / (1024.0 * 1024.0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Linux Disk Error] {ex.Message}");
            }

            return 0;
        }

        private double GetLinuxTotalDisk()
        {
            try
            {
                var output = RunShellCommandSync("df -m /"); 
                var parts = output.Split('\n')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    return double.Parse(parts[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Linux Disk Error] {ex.Message}");
            }

            return 0;
        }

        private string RunShellCommandSync(string cmd)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{cmd}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi);
            return process.StandardOutput.ReadToEnd();
        }
    }
}
