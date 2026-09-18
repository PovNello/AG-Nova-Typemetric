# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]
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
