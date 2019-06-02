using System;
using System.Threading.Tasks;

namespace ArchivialPowerShell.Setup
{
    /// <summary>
    /// Describes functionality for product setup (install, upgrade, and removal).
    /// </summary>
    public interface ISetup
    {
        /// <summary>
        /// Gets the installed version of the product.
        /// </summary>
        /// <returns></returns>
        Task<Version> GetInstalledVersionAsync();

        /// <summary>
        /// Gets the running PowerShell module version.
        /// </summary>
        /// <returns></returns>
        Version GetPowerShellModuleVersion();

        /// <summary>
        /// Checks if this process is running elevated.
        /// </summary>
        /// <returns></returns>
        bool IsRunningElevated();

        /// <summary>
        /// Checks if the SQL Server prerequisite is available.
        /// </summary>
        /// <returns></returns>
        bool SqlServerPrerequisiteIsAvailable();

        /// <summary>
        /// Sets the core application settings.
        /// </summary>
        /// <param name="installationDirectory"></param>
        void CreateCoreSettings(string installationDirectory);

        /// <summary>
        /// Creates a custom event log and event source.
        /// </summary>
        void CreateEventLogSource();

        /// <summary>
        /// Creates the installation directories.
        /// </summary>
        void CreateInstallationDirectories();

        /// <summary>
        /// Copies the program files to the installation directory.
        /// </summary>
        void CopyProgramFiles();

        /// <summary>
        /// Creates the client windows service.
        /// </summary>
        void CreateClientService();

        /// <summary>
        /// Starts the client service.
        /// </summary>
        void StartClientService();

        /// <summary>
        /// Waits until the first time setup/init is completed.
        /// </summary>
        /// <remarks>
        /// First time setup is done once the database is initialized, so check for the required publish flag/option state.
        /// </remarks>
        void WaitForFirstTimeSetup();

        /// <summary>
        /// Stops the client windows service.
        /// </summary>
        void StopClientService();

        /// <summary>
        /// Removes the client windows service.
        /// </summary>
        void DeleteClientService();

        /// <summary>
        /// Removes the installation directories.
        /// </summary>
        void DeleteInstallationDirectories();

        /// <summary>
        /// Removes the event log source.
        /// </summary>
        void DeleteEventLogContents();

        /// <summary>
        /// Removes the core settings.
        /// </summary>
        void DeleteCoreSettings();

        /// <summary>
        /// Sets the database publish required core option to true/enabled.
        /// </summary>
        void SetDatabasePublishRequiredCoreOption();
    }
}
