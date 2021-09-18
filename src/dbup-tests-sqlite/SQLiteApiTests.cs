using System;
using Xunit;

namespace DbUp.Tests.SQLite
{
    public class SQLiteApiTests
    {
        private readonly ApiTests apiTests;

        public SQLiteApiTests()
        {
            apiTests = new ApiTests();
        }

        [Theory]
        [InlineData(typeof(SQLiteExtensions))]
        public void NoPublicApiChanges(Type type, bool differByFramework = false)
        {
            apiTests.NoPublicApiChanges(type, differByFramework);
        }
    }
}
