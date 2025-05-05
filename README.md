# System Monitoring Console App
Cross-platform console application in C# for monitoring system resources, and plugin support for logging, as well as API integration.

# ⚙️ Features

- Monitors:
  - CPU utilization (%)
  - RAM utilization (used / total MB)
  - Disk utilization (used / total MB)
- Plugin architecture:
  - Simply extensible without altering core logic
  - File Logger plugin
- REST API plugin
-  `appsettings.json` configuration
-  Console-based with clean, live updates
-  Cross-platform architecture (Windows-first implementation)

---

# How to Run

1. Install [.NET 6 SDK or later](https://dotnet.microsoft.com/en-us/download).
2. Clone or extract the project:
   git clone https://github.com/your-username/SystemMonitoring.git
   cd SystemMonitoring

# Project Structure
SystemMonitoringApp/
├── Domain/             # Core interfaces and models
├── Infrastructure/     # System metrics implementation
├── Plugins/            # File logger and API plugin
├── Program.cs          # App entry point
├── appsettings.json    # Config file
└── README.md

# Notes
CPU usage is supported for Windows through native .NET APIs.
Clean extension of the architecture is supported for Linux monitoring.
File logger plugin logs system information to a.txt file.
API plugin posts JSON data over HTTP POST to a configurable URL.

# System Monitoring Console App
A C# cross-platform console application for monitoring system resources. The application has support for plugins for logging and API integration, making it easy to extend and customize without altering the core logic.

# Features
System Resource Monitoring:
	- CPU usage: Monitors percentage of CPU usage.
	- RAM usage: Shows used and total memory in MB.
	- Disk usage: Tracks disk usage (used and total in MB).

Plugin Architecture:
	- Easy to extend by adding new plugins without altering core logic.
	- File Logger Plugin: Writes system monitoring information to a.txt file.
	- REST API Plugin: Transmits system monitoring information as JSON over HTTP POST to a configurable endpoint.

Configuration:
	- Configurable options through appsettings.json.

Cross-Platform:
	- Supports Windows with cross-platform compatibility (Linux/macOS) planned.

Console-Based:
	- Prints live system monitoring information to the console window.

# Project Structure

SystemMonitoring/
├── Domain/             # Core interfaces and models for the system.
├── Infrastructure/     # Implementations for fetching system metrics.
├── Plugins/              # Plugin directory (file logger, API plugin).
├── Program.cs          # Entry point of the application.
├── appsettings.json    # Configuration file.
└── README.md        # Project documentation.

# Notes

Windows Support: CPU usage monitoring is implemented for Windows and linux through .NET APIs.
Extensibility: Designed to be readily extendible to macOS system monitoring.
Plugin System: Easily add new monitoring methods or logging mechanisms by implementing plugins without affecting core functionality.

