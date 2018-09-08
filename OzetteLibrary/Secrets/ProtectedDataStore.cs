using OzetteLibrary.Database;
using OzetteLibrary.Exceptions;
using OzetteLibrary.ServiceCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace OzetteLibrary.Secrets
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
        public string GetApplicationSecret(int SecretID)
        {
            // pull encrypted secret from the database.

            var settingValue = Database.GetApplicationOption(SecretID);

            if (string.IsNullOrWhiteSpace(settingValue))
            {
                throw new ApplicationSecretMissingException("Secret ID not found in application store: " + SecretID);
            }

            var settingBytes = Encoding.Default.GetBytes(settingValue);

            // decrypt secret and return value

            var decryptedBytes = ProtectedData.Unprotect(settingBytes, Entropy, Scope);

            return Encoding.Default.GetString(decryptedBytes);
        }

        /// <summary>
        /// Sets the specified secret name/value pair into the secret store.
        /// </summary>
        /// <param name="Option">Application option to save</param>
        public void SetApplicationSecret(ServiceOption Option)
        {
            if (Option == null)
            {
                throw new ArgumentNullException(nameof(Option));
            }
            if (Option.ID <= 0)
            {
                throw new ArgumentException(nameof(Option.ID) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(Option.Name))
            {
                throw new ArgumentException(nameof(Option.Name) + " must be provided.");
            }
            if (string.IsNullOrWhiteSpace(Option.Value))
            {
                throw new ArgumentException(nameof(Option.Value) + " must be provided.");
            }

            // encrypt secret value

            var unencryptedBytes = Encoding.Default.GetBytes(Option.Value);
            var encryptedBytes = ProtectedData.Protect(unencryptedBytes, Entropy, Scope);
            Option.Value = Encoding.Default.GetString(encryptedBytes);

            // store secret in configuration

            Database.SetApplicationOption(Option);
        }
    }
}
