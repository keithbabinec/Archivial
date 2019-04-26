using ArchivialLibrary.Database;
using ArchivialLibrary.Secrets;
using ArchivialPowerShell.Functions.Public;
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

            var command = new GetArchivialNetworkCredentialsCommand(mockedDb.Object);

            foreach (var result in command.Invoke())
            {
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NetCredential));
            }
        }
    }
}
