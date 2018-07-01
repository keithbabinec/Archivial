using OzetteLibrary.ServiceCore;

namespace OzetteLibrary.Secrets
{
    /// <summary>
    /// A generic interface for interacting with application secrets.
    /// </summary>
    public interface ISecretStore
    {
        /// <summary>
        /// Returns the specified secret from the secret store.
        /// </summary>
        /// <param name="SecretName">Name of the secret</param>
        /// <returns>The secret value</returns>
        string GetApplicationSecret(string SecretName);

        /// <summary>
        /// Sets the specified application option into the secret store.
        /// </summary>
        /// <param name="Option">Application option to save</param>
        void SetApplicationSecret(ServiceOption Option);
    }
}
