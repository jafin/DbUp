using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Assent;
using Assent.Namers;
using DbUp.Builder;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.Tests.Common.RecordingDb;
using Shouldly;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using VerifyTests;
using VerifyXunit;

namespace DbUp.Tests.Common
{
    [UsesVerify]
    public abstract class DatabaseSupportTestsBase
    {
        readonly string? parentFilePath;
        readonly IConnectionFactory testConnectionFactory;
        readonly List<SqlScript> scripts = new();
        readonly RecordingDbConnection recordingConnection;
        readonly CaptureLogsLogger logger = new CaptureLogsLogger();

        DatabaseUpgradeResult? result;
        UpgradeEngineBuilder? upgradeEngineBuilder;


        public DatabaseSupportTestsBase([CallerFilePath] string? parentFilePath = null)
        {
            this.parentFilePath = parentFilePath;
            testConnectionFactory = new DelegateConnectionFactory(_ => recordingConnection);
            recordingConnection = new RecordingDbConnection(logger, "SchemaVersions");
        }

        protected abstract UpgradeEngineBuilder DeployTo(SupportedDatabases to);

        protected abstract UpgradeEngineBuilder AddCustomNamedJournalToBuilder(
            UpgradeEngineBuilder builder,
            string schema,
            string tableName
        );

        [BddfyFact]
        public Task VerifyBasicSupport()
        {
            this
                .Given(_ => DeployTo())
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => SingleScriptExists())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .BDDfy();
            return CommandLogReflectsScript(nameof(VerifyBasicSupport));
        }

        [BddfyFact]
        public Task VerifyVariableSubstitutions()
        {
            this
                .Given(_ => DeployTo())
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => SingleScriptWithVariableUsageExists())
                .And(_ => VariableSubstitutionIsSetup())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .BDDfy();

            return CommandLogReflectsScript(nameof(VerifyVariableSubstitutions));
        }

        [BddfyFact]
        public Task VerifyJournalCreationIfNameChanged()
        {
            this
                .Given(_ => DeployTo())
                .And(_ => TargetDatabaseIsEmpty())
                .And(_ => JournalTableNameIsCustomised())
                .And(_ => SingleScriptExists())
                .When(_ => UpgradeIsPerformed())
                .Then(_ => UpgradeIsSuccessful())
                .BDDfy();
            return CommandLogReflectsScript(nameof(VerifyJournalCreationIfNameChanged));
        }


        void VariableSubstitutionIsSetup()
        {
            upgradeEngineBuilder.WithVariable("TestVariable", "SubstitutedValue");
        }

        void JournalTableNameIsCustomised()
        {
            upgradeEngineBuilder = AddCustomNamedJournalToBuilder(upgradeEngineBuilder!, "test", "TestSchemaVersions");
        }


        Task CommandLogReflectsScript(string testName)
        {
            var configuration = new Configuration()
                .UsingSanitiser(Scrubbers.ScrubDates)
                .UsingNamer(new SubdirectoryNamer("ApprovalFiles"));

            // Automatically approve the change, make sure to check the result before committing
            // configuration = configuration.UsingReporter((received, approved) => File.Copy(received, approved, true));

            var settings = new VerifySettings();
            settings.UseDirectory("ApprovalFiles");
            //settings.UniqueForTargetFramework();
            settings.ScrubLinesWithReplace(x =>
            {
                Regex r = new Regex("applied=.*");
                if (r.IsMatch(x))
                    return Regex.Replace(x, "applied=.*", "applied=#DATE#");
                return x;
            });
            return Verifier.Verify(logger.Log, settings, sourceFile: parentFilePath!);
        }

        void UpgradeIsSuccessful()
        {
            result!.Successful.ShouldBe(true);
        }

        void UpgradeIsPerformed()
        {
            result = upgradeEngineBuilder!.Build().PerformUpgrade();
        }

        void SingleScriptExists()
        {
            scripts.Add(new SqlScript("Script0001.sql", "script1contents"));
        }

        void SingleScriptWithVariableUsageExists()
        {
            scripts.Add(new SqlScript("Script0001.sql", "print $TestVariable$"));
        }

        void TargetDatabaseIsEmpty()
        {
        }

        void DeployTo()
        {
            upgradeEngineBuilder = DeployTo(DeployChanges.To)
                .WithScripts(scripts)
                .OverrideConnectionFactory(testConnectionFactory)
                .LogTo(logger);
        }
    }
}
