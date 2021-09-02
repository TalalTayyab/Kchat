using DbUp;
using System;
using System.Linq;
using System.Reflection;

namespace kchat.dbUp.migrations
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("starting migrations...");

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            connectionString = connectionString ?? @"server=localhost;database=db;trusted_connection=true;";

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
