# Architecture

Typemetric is built as a .NET 10 WPF application, prioritizing low resource usage and non-intrusive background operation.

## Core Modules

### 1. GlobalHooks (`Core/GlobalHooks.cs`)
- Uses Windows low-level P/Invoke APIs (`user32.dll`) to set up `WH_KEYBOARD_LL` (13) and `WH_MOUSE_LL` (14) hooks.
- Captures system-wide keystrokes and mouse clicks regardless of which window is in focus.
- Translates Virtual Key codes to WPF `Key` enums.

### 2. MetricsTracker (`Core/MetricsTracker.cs`)
- Acts as an in-memory buffer and thread-safe aggregation engine.
- Intercepts raw hits from `GlobalHooks` and groups them by key/button in a `Dictionary`.
- Flushes the totals to the `DatabaseManager` asynchronously every 60 seconds to minimize disk I/O and prevent locking.

### 3. StartupManager (`Core/StartupManager.cs`)
- Manages the application's boot persistence while maintaining portable design principles.
- Creates or removes a shortcut (`.lnk`) in the user's Startup folder (`%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup`) using `WScript.Shell` via reflection.

### 4. TrayIconManager (`UI/TrayIconManager.cs`)
- Implements `H.NotifyIcon.Wpf` to run the app as a system tray icon.
- Handles the window lifecycle (minimizing to tray instead of exiting) and provides the double-click handler to open the dashboard.

## Memory Management

### RAM Trimming Strategy
Since WPF applications generally reserve a large memory footprint, Typemetric forces a memory flush whenever the main window is hidden.
- It calls `App.TrimMemory()`, which uses `SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)`.
- This violently drops unused managed memory pages to the OS pagefile, reducing background active RAM to roughly ~15MB.
