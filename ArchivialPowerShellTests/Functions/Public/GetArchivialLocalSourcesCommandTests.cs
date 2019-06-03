using ArchivialLibrary.Database;
using ArchivialLibrary.Folders;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class GetArchivialLocalSourcesCommandTests
    {
        [TestMethod]
        public void GetArchivialLocalSourcesCommand_CanReturnLocalSources()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetSourceLocationsAsync())
                .ReturnsAsync(new SourceLocations()
                {
                    new LocalSourceLocation()
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new GetArchivialLocalSourcesCommand(depedencies);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(LocalSourceLocation));
            }
        }

        [TestMethod]
        public void GetArchivialLocalSourcesCommand_DoesNotReturnNetworkSources()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetSourceLocationsAsync())
                .ReturnsAsync(new SourceLocations()
                {
                    new NetworkSourceLocation()
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new GetArchivialLocalSourcesCommand(depedencies);

            foreach (var result in command.Invoke())
            {
                Assert.Fail("Expected no results, but at least one result was returned.");
            }
        }
    }
}
