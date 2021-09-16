using System;
using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Engine;

namespace TestApp
{
    class Program
    {
        static int Main(string[] args)
        {
            //var result = MySql();
            var result = SqLite();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
            }

            Console.ReadLine();
            return 0;
        }

        private static DatabaseUpgradeResult SqLite()
        {
            var connectionString = "Data source=:memory:";

            var upgrader =
                DeployChanges.To
                    .SQLiteDatabase(connectionString)
                    .WithScript("test", @"
create table test (
    contact_id INTEGER PRIMARY KEY
);
")
                    .LogScriptOutput()
                    .LogToConsole()
                    .Build();
            return upgrader.PerformUpgrade();
        }

        private static DatabaseUpgradeResult MySql()
        {
            var connectionString = "server=localhost;uid=root;pwd=z;database=dbup;SSL Mode=None";
            // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // var enc1252 = Encoding.GetEncoding(1252);

            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            TestParser parser = new TestParser();
            parser.Parse();

            var upgrader =
                DeployChanges.To
                    .MySqlDatabase(connectionString)
                    //.SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogScriptOutput()
                    .LogToConsole()
                    .Build();
            var result = upgrader.PerformUpgrade();
            return result;
        }
    }
}
