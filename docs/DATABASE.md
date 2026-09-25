# Database & Storage

Typemetric uses a local `SQLite` database (`metrics.db`) to persist telemetry.
The absolute path is determined by the `AppConfig` and can be chosen by the user during the first-run Setup Wizard.
To ensure data integrity and true portability, all database transactions are simultaneously mirrored to a local backup file (`metrics_backup.db`) stored directly in the application's base directory.

## Privacy & Security Constraints

- **No Logging**: Typemetric must NEVER log the sequences, timing, or context of keystrokes.
- **Aggregation**: The application records only the atomic *frequency* (Count) of a specific key pressed on a specific date.
- No network requests or telemetry transmissions are permitted.

## Schema Overview

The database uses three main tables to segment inputs:

### `KeyStats`
| Column | Type | Description |
|---|---|---|
| `Date` | TEXT | Format `yyyy-MM-dd` (Primary Key part 1) |
| `KeyName` | TEXT | String representation of WPF `Key` enum (Primary Key part 2) |
| `Count` | INTEGER | The number of times the key was pressed |

### `MouseStats`
| Column | Type | Description |
|---|---|---|
| `Date` | TEXT | Format `yyyy-MM-dd` (Primary Key part 1) |
| `ButtonName` | TEXT | "Left", "Right", "Middle" (Primary Key part 2) |
| `Count` | INTEGER | The number of times the button was clicked |

### `ScrollStats`
| Column | Type | Description |
|---|---|---|
| `Date` | TEXT | Format `yyyy-MM-dd` (Primary Key part 1) |
| `Direction` | TEXT | "Up", "Down" (Primary Key part 2) |
| `Ticks` | INTEGER | Raw Windows `WHEEL_DELTA` value. Usually 1 physical click = 120 ticks |

## Querying

- **Daily Data**: Queries filter `WHERE Date = $date` to show today's statistics.
- **All-Time Data**: Queries use `SUM(Count)` and `GROUP BY KeyName` / `ButtonName` to construct historical frequencies ignoring dates.
