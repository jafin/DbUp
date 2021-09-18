using DbUp.SQLite;
using TestStack.BDDfy;
using Xunit;

namespace DbUp.Tests.SQLite
{
    public class DatabaseSupportTests : Tests.DatabaseSupportTests
    {
        public DatabaseSupportTests()
        {
            base.SetCallerFilePath();
        }

        [Fact]
        public override void VerifyBasicSupport()
        {
            base.VerifyBasicSupport();
        }

        [Fact]
        public override void VerifyVariableSubstitutions()
        {
            base.VerifyVariableSubstitutions();
        }

        [Fact]
        public override void VerifyJournalCreationIfNameChanged()
        {
            base.VerifyJournalCreationIfNameChanged();
        }

        public override ExampleTable DatabaseExampleTable => new ExampleTable("Deploy to")
        {
            new ExampleAction("SQLite", Deploy(to => to.SQLiteDatabase(string.Empty), (builder, schema, tableName) =>
            {
                builder.Configure(c => c.Journal = new SQLiteTableJournal(() => c.ConnectionManager, () => c.Log, tableName));
                return builder;
            }))
        };
    }
}
