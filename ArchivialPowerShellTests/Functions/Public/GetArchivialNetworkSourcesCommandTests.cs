using ArchivialLibrary.Database;
using ArchivialLibrary.Folders;
using ArchivialPowerShell.Functions.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class GetArchivialNetworkSourcesCommandTests
    {
        [TestMethod]
        public void GetArchivialNetworkSourcesCommand_CanReturnNetworkSources()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetSourceLocationsAsync())
                .ReturnsAsync(new SourceLocations()
                {
                    new NetworkSourceLocation()
                });

            var command = new GetArchivialNetworkSourcesCommand(mockedDb.Object);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NetworkSourceLocation));
            }
        }

        [TestMethod]
        public void GetArchivialNetworkSourcesCommand_DoesNotReturnLocalSources()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetSourceLocationsAsync())
                .ReturnsAsync(new SourceLocations()
                {
                    new LocalSourceLocation()
                });

            var command = new GetArchivialNetworkSourcesCommand(mockedDb.Object);

            foreach (var result in command.Invoke())
            {
                Assert.Fail("Expected no results, but at least one result was returned.");
            }
        }
    }
}
