using System.Security.Cryptography;
using System.IO;

namespace OzetteLibrary.Crypto
{
    /// <summary>
    /// Contains functionality for hashing.
    /// </summary>
    public class Hasher
    {
        /// <summary>
        /// Generates a 20-byte long file hash.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] Generate20ByteFileHash(string filePath)
        {
            return GenerateFileHash(HashAlgorithmName.SHA1, filePath);
        }

        /// <summary>
        /// Generates a 32-byte long file hash.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] Generate32ByteFileHash(string filePath)
        {
            return GenerateFileHash(HashAlgorithmName.SHA256, filePath);
        }

        /// <summary>
        /// Generates a 64-byte long file hash.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] Generate64ByteFileHash(string filePath)
        {
            return GenerateFileHash(HashAlgorithmName.SHA512, filePath);
        }

        /// <summary>
        /// Generates a file hash using the specified hash algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm to use.</param>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        private byte[] GenerateFileHash(HashAlgorithmName hashAlgorithm, string filePath)
        {
            using (FileStream fs = System.IO.File.OpenRead(filePath))
            {
                using (var hasher = HashAlgorithm.Create(hashAlgorithm.Name))
                {
                    return hasher.ComputeHash(fs);
                }
            }
        }
    }
}
