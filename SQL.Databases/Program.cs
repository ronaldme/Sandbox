using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SQL.Databases
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbNames = GetDatabaseNames();

            var systemDbs = new[] {"master", "tempdb", "model", "msdb"};
            dbNames.RemoveAll(s => systemDbs.Contains(s));

            dbNames.ForEach(Console.WriteLine);

            Console.ReadLine();
        }

        private static List<string> GetDatabaseNames()
        {
            var dbNames = new List<string>();

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["sql-server-db"].ConnectionString))
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