using System;
using DbUp.MySql;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.MySql.Parser
{
    public class MySqlParserTests
    {
        private int commandCount = 0;

        [Fact]
        public void Parse()
        {
            MySqlCommandReader reader = new MySqlCommandReader(@"
DROP PROCEDURE IF EXISTS My_StoreProc;

CREATE PROCEDURE My_StoreProc(strEmail VARCHAR(255))
BEGIN
SELECT id FROM table_name WHERE email = strEmail
ORDER BY id DESC limit 1;
END;
");
            reader.ReadAllCommands(CommandHandler);
            commandCount.ShouldBe(2);
        }

        private void CommandHandler(string val)
        {
            Console.WriteLine(val);
            commandCount++;
        }
    }
}
