using System;
using System.Collections.Generic;
using com.protectsoft.SqlStatementParser;

namespace TestApp
{
    public class TestParser
    {
        public void Parse()
        {
            var sql = @"
CREATE PROCEDURE My_StoreProc(strEmail VARCHAR(255))
BEGIN
SELECT id FROM table_name WHERE email = strEmail
ORDER BY id DESC limit 1;
END;";
            var parser = new SqlStatementParserWrapper(sql, DbType.MYSQL);
            List<string> statements = SqlStatementParserWrapper.convert(parser.sql, parser.Parse());
            foreach (var statement in statements)
            {
                Console.WriteLine(statement);
            }
        }
    }
}
