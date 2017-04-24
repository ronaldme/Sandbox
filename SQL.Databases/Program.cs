using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SQL.Databases
{
    class Program
    {
        private static readonly string dbPath = @"D:\databases";
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["sql-server-db"].ConnectionString;

        static void Main(string[] args)
        {
            var dbNames = GetDatabaseNames();

            var systemDbs = new[] {"master", "tempdb", "model", "msdb"};
            dbNames.RemoveAll(s => systemDbs.Contains(s));

            dbNames.ForEach(Console.WriteLine);

            Console.WriteLine("Starting to backup databases...");
            BackupDatabases(dbNames);

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void BackupDatabases(List<string> databaseNames)
        {
            var stopWatch = Stopwatch.StartNew();
            foreach (var dbName in databaseNames)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    Directory.CreateDirectory($@"{dbPath}\{dbName}");

                    var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
                    var cmd = new SqlCommand
                    {
                        CommandText = $@"BACKUP DATABASE ""{dbName}"" TO DISK = '{dbPath}\{dbName}\{date}-{dbName}.bak'",
                        CommandType = CommandType.Text,
                        Connection = connection
                    };

                    connection.Open();
                    cmd.ExecuteReader();
                    connection.Close();
                }
            }
            Console.WriteLine($"Created database backups in: {stopWatch.ElapsedMilliseconds}ms.");
        }

        private static List<string> GetDatabaseNames()
        {
            var dbNames = new List<string>();

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var databases = con.GetSchema("Databases");

                foreach (DataRow database in databases.Rows)
                {
                    string databaseName = database.Field<string>("database_name");
                    dbNames.Add(databaseName);
                }
            }

            return dbNames;
        }
    }
}