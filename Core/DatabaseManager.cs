using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Typemetric.Core
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _backupConnectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            string backupPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metrics_backup.db");
            _backupConnectionString = $"Data Source={backupPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            string schema = @"
                CREATE TABLE IF NOT EXISTS KeyStats (
                    Date TEXT NOT NULL,
                    KeyName TEXT NOT NULL,
                    Count INTEGER DEFAULT 1,
                    PRIMARY KEY (Date, KeyName)
                );
                CREATE TABLE IF NOT EXISTS MouseStats (
                    Date TEXT NOT NULL,
                    ButtonName TEXT NOT NULL,
                    Count INTEGER DEFAULT 1,
                    PRIMARY KEY (Date, ButtonName)
                );
                CREATE TABLE IF NOT EXISTS ScrollStats (
                    Date TEXT NOT NULL,
                    Direction TEXT NOT NULL,
                    Ticks INTEGER DEFAULT 1,
                    PRIMARY KEY (Date, Direction)
                );
            ";

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = schema;
                command.ExecuteNonQuery();
            }

            using (var connection = new SqliteConnection(_backupConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = schema;
                command.ExecuteNonQuery();
            }
        }

        public void IncrementKeyCounts(string date, Dictionary<string, int> keys)
        {
            Action<string> execute = (connString) =>
            {
                try
                {
                    using var connection = new SqliteConnection(connString);
                    connection.Open();
                    using var transaction = connection.BeginTransaction();

                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO KeyStats (Date, KeyName, Count)
                        VALUES ($date, $keyName, $count)
                        ON CONFLICT(Date, KeyName) DO UPDATE SET Count = Count + $count;
                    ";
                    
                    var dateParam = command.Parameters.Add("$date", SqliteType.Text);
                    var keyParam = command.Parameters.Add("$keyName", SqliteType.Text);
                    var countParam = command.Parameters.Add("$count", SqliteType.Integer);

                    dateParam.Value = date;

                    foreach (var kvp in keys)
                    {
                        keyParam.Value = kvp.Key;
                        countParam.Value = kvp.Value;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch { }
            };

            execute(_connectionString);
            execute(_backupConnectionString);
        }        
        public void IncrementMouseCounts(string date, Dictionary<string, int> buttons)
        {
            Action<string> execute = (connString) =>
            {
                try
                {
                    using var connection = new SqliteConnection(connString);
                    connection.Open();
                    using var transaction = connection.BeginTransaction();

                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO MouseStats (Date, ButtonName, Count)
                        VALUES ($date, $buttonName, $count)
                        ON CONFLICT(Date, ButtonName) DO UPDATE SET Count = Count + $count;
                    ";
                    
                    var dateParam = command.Parameters.Add("$date", SqliteType.Text);
                    var btnParam = command.Parameters.Add("$buttonName", SqliteType.Text);
                    var countParam = command.Parameters.Add("$count", SqliteType.Integer);

                    dateParam.Value = date;

                    foreach (var kvp in buttons)
                    {
                        btnParam.Value = kvp.Key;
                        countParam.Value = kvp.Value;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch { }
            };

            execute(_connectionString);
            execute(_backupConnectionString);
        }
        public void IncrementScrollTicks(string date, int delta)
        {
            string direction = delta > 0 ? "Up" : "Down";
            int ticks = Math.Abs(delta);

            Action<string> execute = (connString) =>
            {
                try
                {
                    using var connection = new SqliteConnection(connString);
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO ScrollStats (Date, Direction, Ticks)
                        VALUES ($date, $direction, $ticks)
                        ON CONFLICT(Date, Direction) DO UPDATE SET Ticks = Ticks + $ticks;
                    ";
                    command.Parameters.AddWithValue("$date", date);
                    command.Parameters.AddWithValue("$direction", direction);
                    command.Parameters.AddWithValue("$ticks", ticks);
                    command.ExecuteNonQuery();
                }
                catch { }
            };

            execute(_connectionString);
            execute(_backupConnectionString);
        }

        public (int Keystrokes, int MouseClicks, int ScrollTicks) GetTodayTotals(string date)
        {
            int keys = 0;
            int mouse = 0;
            int scroll = 0;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            
            cmd.CommandText = "SELECT SUM(Count) FROM KeyStats WHERE Date = $date";
            cmd.Parameters.AddWithValue("$date", date);
            var keyRes = cmd.ExecuteScalar();
            if (keyRes != DBNull.Value && keyRes != null) keys = Convert.ToInt32(keyRes);

            cmd.CommandText = "SELECT SUM(Count) FROM MouseStats WHERE Date = $date";
            var mouseRes = cmd.ExecuteScalar();
            if (mouseRes != DBNull.Value && mouseRes != null) mouse = Convert.ToInt32(mouseRes);

            cmd.CommandText = "SELECT SUM(Ticks) FROM ScrollStats WHERE Date = $date";
            var scrollRes = cmd.ExecuteScalar();
            if (scrollRes != DBNull.Value && scrollRes != null) scroll = Convert.ToInt32(scrollRes);

            return (keys, mouse, scroll);
        }

        public (int Keystrokes, int MouseClicks, int ScrollTicks) GetAllTimeTotals()
        {
            int keys = 0, mouse = 0, scroll = 0;
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Count) FROM KeyStats";
            var k = cmd.ExecuteScalar(); if (k != System.DBNull.Value && k != null) keys = System.Convert.ToInt32(k);

            cmd.CommandText = "SELECT SUM(Count) FROM MouseStats";
            var m = cmd.ExecuteScalar(); if (m != System.DBNull.Value && m != null) mouse = System.Convert.ToInt32(m);

            cmd.CommandText = "SELECT SUM(Ticks) FROM ScrollStats";
            var s = cmd.ExecuteScalar(); if (s != System.DBNull.Value && s != null) scroll = System.Convert.ToInt32(s);

            return (keys, mouse, scroll);
        }

        public System.Collections.Generic.Dictionary<string, int> GetFrequenciesForDate(string date)
        {
            var freqs = new System.Collections.Generic.Dictionary<string, int>();
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT KeyName, Count FROM KeyStats WHERE Date = $date";
            cmd.Parameters.AddWithValue("$date", date);
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                freqs[reader.GetString(0)] = reader.GetInt32(1);
            }
            return freqs;
        }

        public System.Collections.Generic.Dictionary<string, int> GetAllTimeFrequencies()
        {
            var freqs = new System.Collections.Generic.Dictionary<string, int>();
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT KeyName, SUM(Count) FROM KeyStats GROUP BY KeyName";
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                freqs[reader.GetString(0)] = reader.GetInt32(1);
            }
            return freqs;
        }

        public System.Collections.Generic.Dictionary<string, int> GetMouseFrequenciesForDate(string date)
        {
            var freqs = new System.Collections.Generic.Dictionary<string, int>();
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ButtonName, Count FROM MouseStats WHERE Date = $date";
            cmd.Parameters.AddWithValue("$date", date);
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                freqs[reader.GetString(0)] = reader.GetInt32(1);
            }
            return freqs;
        }

        public System.Collections.Generic.Dictionary<string, int> GetAllTimeMouseFrequencies()
        {
            var freqs = new System.Collections.Generic.Dictionary<string, int>();
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ButtonName, SUM(Count) FROM MouseStats GROUP BY ButtonName";
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                freqs[reader.GetString(0)] = reader.GetInt32(1);
            }
            return freqs;
        }
    }
}
