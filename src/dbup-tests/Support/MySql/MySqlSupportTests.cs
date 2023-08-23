using System.Threading.Tasks;
using DbUp.Tests.Common;
using DbUp.Tests.Common.RecordingDb;
using Shouldly;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Scrubbers = DbUp.Tests.Common.Scrubbers;

namespace DbUp.Tests.Support.MySql
{
    [UsesVerify]
    public class MySqlSupportTests
    {
        [Fact]
        public Task CanHandleDelimiter()
        {
            var logger = new CaptureLogsLogger();
            var recordingDbConnection = new RecordingDbConnection(logger, "schemaversions");
            recordingDbConnection.SetupRunScripts();
            var upgrader = DeployChanges.To
                .MySqlDatabase(string.Empty)
                .OverrideConnectionFactory(recordingDbConnection)
                .LogTo(logger)
                .WithScript("Script0003", @"USE `test`;
DROP procedure IF EXISTS `testSproc`;

DELIMITER $$

USE `test`$$
CREATE PROCEDURE `testSproc`(
        IN   ssn                    VARCHAR(32)
     )
BEGIN

    SELECT id
    FROM   customer as c
    WHERE  c.ssn = ssn ;

END$$").Build();

            var result = upgrader.PerformUpgrade();

            result.Successful.ShouldBe(true);
            return Verifier.Verify(logger.Log, GetVerifySettings());
        }

        private VerifySettings GetVerifySettings()
        {
            VerifySettings settings = new();
            settings.UseDirectory("ApprovalFiles");
            settings.ScrubLinesWithReplace(Scrubbers.ScrubDates);
            return settings;
        }
    }
}
