using ArchivialLibrary.Database;
using ArchivialLibrary.Providers;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class GetArchivialProvidersCommandTests
    {
        [TestMethod]
        public void GetArchivialProvidersCommand_CanReturnProviders()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetProvidersAsync(It.IsAny<ProviderTypes>()))
                .ReturnsAsync(new ProviderCollection()
                {
                    new Provider()
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new GetArchivialProvidersCommand(depedencies);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(Provider));
            }
        }
    }
}
