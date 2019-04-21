using ArchivialLibrary.Database;
using ArchivialLibrary.Exceptions;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ArchivialLibrary.Secrets
{
    /// <summary>
    /// An <c>ISecretStore</c> implementation that uses the Windows data protection API.
    /// </summary>
    public class ProtectedDataStore : ISecretStore
    {
        /// <summary>
        /// A reference to the database where settings are stored.
        /// </summary>
        private IClientDatabase Database { get; set; }

        /// <summary>
        /// Data protection scope (machine vs user) for the data protection API.
        /// </summary>
        private DataProtectionScope Scope { get; set; }

        /// <summary>
        /// Additional entropy for the data protection API.
        /// </summary>
        private byte[] Entropy { get; set; }

        /// <summary>
        /// A constructor that specifies data protection scope and entropy.
        /// </summary>
        /// <param name="settingsDatabase">Settings database</param>
        /// <param name="scope">Data protection scope</param>
        /// <param name="entropy">Additional entropy</param>
        public ProtectedDataStore(IClientDatabase settingsDatabase, DataProtectionScope scope, byte[] entropy)
        {
            if (settingsDatabase == null)
            {
                throw new ArgumentNullException(nameof(settingsDatabase));
            }
            if (entropy == null || entropy.Length == 0)
            {
                throw new ArgumentException(nameof(entropy) + " value must be provided.");
            }

            Database = settingsDatabase;
            Scope = scope;
            Entropy = entropy;
        }

        /// <summary>
        /// Returns the specified secret from the secret store.
        /// </summary>
        /// <param name="SecretID">ID of the secret</param>
        /// <returns>The secret value</returns>
        public async Task<string> GetApplicationSecretAsync(string SecretName)
        {
            // pull encrypted secret from the database.

            var settingValue = await Database.GetApplicationOptionAsync(SecretName).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(settingValue))
            {
                throw new ApplicationSecretMissingException("Secret not found in application store: " + SecretName);
            }

            var settingBytes = Encoding.Default.GetBytes(settingValue);

            // decrypt secret and return value

            var decryptedBytes = ProtectedData.Unprotect(settingBytes, Entropy, Scope);

            return Encoding.Default.GetString(decryptedBytes);
        }

        /// <summary>
        /// Sets the specified secret into the secret store.
        /// </summary>
        /// <param name="SecretName"></param>
        /// <param name="SecretValue"></param>
        public async Task SetApplicationSecretAsync(string SecretName, string SecretValue)
        {
            if (string.IsNullOrWhiteSpace(SecretName))
            {
                throw new ArgumentException(nameof(SecretName) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(SecretValue))
            {
                throw new ArgumentException(nameof(SecretValue) + " must be provided.");
            }

            // encrypt secret value

            var unencryptedBytes = Encoding.Default.GetBytes(SecretValue);
            var encryptedBytes = ProtectedData.Protect(unencryptedBytes, Entropy, Scope);
            var encryptedString = Encoding.Default.GetString(encryptedBytes);

            // store secret in configuration

            await Database.SetApplicationOptionAsync(SecretName, encryptedString).ConfigureAwait(false);
        }
    }
}
