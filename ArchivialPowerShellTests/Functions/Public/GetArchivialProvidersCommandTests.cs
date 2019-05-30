using ArchivialLibrary.Database;
using ArchivialLibrary.Providers;
using ArchivialPowerShell.Functions.Public;
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

            var command = new GetArchivialProvidersCommand(mockedDb.Object, null, null);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(Provider));
            }
        }
    }
}
