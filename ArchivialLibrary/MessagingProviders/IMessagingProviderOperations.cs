using OzetteLibrary.ServiceCore;
using System.Threading.Tasks;

namespace OzetteLibrary.MessagingProviders
{
    /// <summary>
    /// Describes the required operations for messaging providers.
    /// </summary>
    public interface IMessagingProviderOperations
    {
        /// <summary>
        /// Sends the backup progress status to the messaging provider for delivery.
        /// </summary>
        /// <param name="Progress">An initialized <c>BackupProgress</c> object.</param>
        Task SendBackupProgressStatusMessageAsync(BackupProgress Progress);
    }
}
