using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace DbUp.Tests.SQLite
{
    public class ApiTests
    {
        private readonly Tests.ApiTests apiTests;

        public ApiTests()
        {
            apiTests = new Tests.ApiTests();
        }

        [Theory]
        [InlineData(typeof(SQLiteExtensions))]
        public void NoPublicApiChanges(Type type, bool differByFramework = false)
        {
            apiTests.NoPublicApiChanges(type, differByFramework, GetCallerFilePath());
        }

        protected string GetCallerFilePath([CallerFilePath] string path = null)
        {
            return path;
        }
    }
}
