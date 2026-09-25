# Changelog

All notable changes to this project will be documented in this file.
## [0.0.2] - 2026-09-25
### Changed
- Converted application to be 100% portable by moving `config.json` saving logic from `%LocalAppData%` to the app's root directory.
- Replaced Windows Registry startup method with a portable-friendly Startup folder shortcut (`.lnk`) mechanism.
- Enhanced Database Manager to mirror all telemetry transactions to a local `metrics_backup.db` within the app folder.
- Expanded Setup Wizard into two steps to handle database location and portable startup preferences.
- Added database recovery logic to prompt for restore if the primary database is missing but the local backup exists.

## [0.0.1] - 2026-09-20
### Added
- Setup Wizard on first run to choose SQLite database location.
- GlobalHooks implementation to capture system-wide keystrokes and mouse clicks.
- Real-time `VisualKeyboard` and `VisualMouse` UI for the Heatmap.
- Continuous Logarithmic RGB interpolation for the Heatmap to normalize frequencies.
- Startup Manager to optionally launch the app on Windows boot via Registry.
- Toggle between Today and All-Time frequency visualization.
- System Tray integration for silent background running.

### Fixed
- Fixed WPF Window clipping issue by removing hardcoded `RectangleGeometry` mask and applying dynamic `SizeToContent`.
- Fixed Database schema typings where queries expected integer IDs but the DB stored string names (`KeyName`, `ButtonName`).
- Prevented extreme RAM usage by forcing `TrimMemory` (SetProcessWorkingSetSize) when the Window minimizes to the tray.
