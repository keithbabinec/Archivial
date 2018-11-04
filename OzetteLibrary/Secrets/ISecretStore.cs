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
        /// <param name="SecretID">ID of the secret</param>
        /// <returns>The secret value</returns>
        string GetApplicationSecret(string SecretName);

        /// <summary>
        /// Sets the specified secret into the secret store.
        /// </summary>
        /// <param name="SecretName"></param>
        /// <param name="SecretValue"></param>
        void SetApplicationSecret(string SecretName, string SecretValue);
    }
}
