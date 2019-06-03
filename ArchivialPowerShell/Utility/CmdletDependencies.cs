using ArchivialLibrary.Database;
using ArchivialLibrary.Secrets;
using ArchivialLibrary.ServiceCore;
using ArchivialPowerShell.Setup;

namespace ArchivialPowerShell.Utility
{
    /// <summary>
    /// Contains the objects required for dependency injection into the base cmdlet.
    /// </summary>
    public class CmdletDependencies
    {
        /// <summary>
        /// A reference to an initialized or mocked client database.
        /// </summary>
        public IClientDatabase ClientDatabase { get; set; }

        /// <summary>
        /// A reference to an initialized or mocked secret store.
        /// </summary>
        public ISecretStore SecretStore { get; set; }

        /// <summary>
        /// A reference to an initialized or mocked setup helper class.
        /// </summary>
        public ISetup Setup { get; set; }

        /// <summary>
        /// A reference to an initialized or mocked core settings accessor.
        /// </summary>
        public ICoreSettings CoreSettings { get; set; }
    }
}
