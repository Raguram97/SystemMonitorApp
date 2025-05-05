# 🖥️ System Monitoring Console App

A cross-platform console application written in C# that monitors system resources and supports plugins for logging and API integration. Designed using clean architecture principles, it’s lightweight, extensible, and ideal for real-time system diagnostics.

---

## ⚙️ Features

### 🔍 System Resource Monitoring
- **CPU Usage** – Real-time percentage of CPU consumption.
- **RAM Usage** – Used and total memory in MB.
- **Disk Usage** – Used and total disk space in MB.

### 🧩 Plugin Architecture
- Modular and extensible design.
- **File Logger Plugin** – Logs data to a `.txt` file.
- **REST API Plugin** – Sends data as JSON to a configurable HTTP endpoint.

### 📁 Configuration
- Centralized configuration using `appsettings.json`.

### 🧼 Clean Console UI
- Live updates in an easy-to-read format.

### 🌐 Cross-Platform Ready
- Windows-first implementation with a structure supporting Linux/macOS extensions.

---

## 🧪 Getting Started

### Prerequisites
- [.NET 6 SDK or later](https://dotnet.microsoft.com/en-us/download)

## 📦 Dependencies
This project requires the following NuGet packages.

```bash
dotnet add package Microsoft.Extensions.Configuration --version 9.0.4
dotnet add package Microsoft.Extensions.Configuration.Binder --version 9.0.4
dotnet add package Microsoft.Extensions.Configuration.Json --version 9.0.4
dotnet add package Microsoft.Extensions.DependencyInjection --version 9.0.4
dotnet add package Microsoft.Extensions.Options --version 9.0.4
dotnet add package Microsoft.VisualBasic --version 10.3.0
dotnet add package System.Diagnostics.PerformanceCounter --version 9.0.4
dotnet add package System.Management --version 9.0.4
```
## 📁 Project Structure
```bash
SystemMonitoring/
├── Domain/             # Core interfaces and domain models
├── Infrastructure/     # System metrics logic
├── Plugins/            # File logger and REST API plugin
├── Program.cs          # Application entry point
├── appsettings.json    # App configuration
└── README.md           # Project documentation
```
### Running the App

```bash
git clone https://github.com/Raguram97/SystemMonitorApp.git
cd SystemMonitoring
dotnet build
dotnet run
```
