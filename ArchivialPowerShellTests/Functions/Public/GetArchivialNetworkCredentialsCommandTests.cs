using ArchivialLibrary.Database;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Functions.Public;
using ArchivialPowerShell.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ArchivialPowerShellTests.Functions.Public
{
    [TestClass]
    public class GetArchivialNetworkCredentialsCommandTests
    {
        [TestMethod]
        public void GetArchivialNetworkCredentialsCommand_CanReturnNetCreds()
        {
            var mockedDb = new Mock<IClientDatabase>();

            mockedDb.Setup(x => x.GetNetCredentialsAsync())
                .ReturnsAsync(new NetCredentialsCollection()
                {
                    new NetCredential()
                });

            var mockedCoreSettings = new Mock<ICoreSettings>();

            var depedencies = new CmdletDependencies()
            {
                ClientDatabase = mockedDb.Object,
                CoreSettings = mockedCoreSettings.Object
            };

            var command = new GetArchivialNetworkCredentialsCommand(depedencies);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NetCredential));
            }
        }
    }
}
