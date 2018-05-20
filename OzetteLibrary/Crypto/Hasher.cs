using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using OzetteLibrary.Models;
using OzetteLibrary.Logging;

namespace OzetteLibrary.Crypto
{
    /// <summary>
    /// Contains functionality for hashing.
    /// </summary>
    public class Hasher
    {
        public Hasher(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        private ILogger Logger { get; set; }

        /// <summary>
        /// Generates the default file hash for the specified priority.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <param name="priority">FileBackupPriority</param>
        /// <returns></returns>
        public byte[] GenerateDefaultHash(string filePath, FileBackupPriority priority)
        {
            if (priority == FileBackupPriority.Low)
            {
                return Generate20ByteFileHash(filePath);
            }
            else if (priority == FileBackupPriority.Medium)
            {
                return Generate32ByteFileHash(filePath);
            }
            else if (priority == FileBackupPriority.High)
            {
                return Generate64ByteFileHash(filePath);
            }
            else
            {
                throw new NotImplementedException("Unexpected file backup priority type: " + priority);
            }
        }

        /// <summary>
        /// Generates a 20-byte long file hash.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] Generate20ByteFileHash(string filePath)
        {
            return HashCompleteFileFromFilePath(HashAlgorithmName.SHA1, filePath);
        }

        /// <summary>
        /// Generates a 32-byte long file hash.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] Generate32ByteFileHash(string filePath)
        {
            return HashCompleteFileFromFilePath(HashAlgorithmName.SHA256, filePath);
        }

        /// <summary>
        /// Generates a 64-byte long file hash.
        /// </summary>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] Generate64ByteFileHash(string filePath)
        {
            return HashCompleteFileFromFilePath(HashAlgorithmName.SHA512, filePath);
        }

        /// <summary>
        /// Generates a file hash using the specified hash algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm to use.</param>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] HashCompleteFileFromFilePath(HashAlgorithmName hashAlgorithm, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(nameof(filePath));
            }

            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    using (var hasher = HashAlgorithm.Create(hashAlgorithm.Name))
                    {
                        return hasher.ComputeHash(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                // log the error
                Logger.WriteTraceError("Failed to generate a hash for file: " + filePath, ex, Logger.GenerateFullContextStackTrace());

                // return empty byte array
                return new byte[] { };
            }
        }

        /// <summary>
        /// Generates a hash for a byte array using the specified algorithm.
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="streamBytes"></param>
        /// <returns></returns>
        public byte[] HashFileBlockFromByteArray(HashAlgorithmName hashAlgorithm, byte[] streamBytes)
        {
            if (streamBytes == null || streamBytes.Length == 0)
            {
                throw new ArgumentException(nameof(streamBytes));
            }

            try
            {
                using (var hasher = HashAlgorithm.Create(hashAlgorithm.Name))
                {
                    return hasher.ComputeHash(streamBytes);
                }
            }
            catch (Exception ex)
            {
                // log the error
                Logger.WriteTraceError("Failed to generate a hash for the byte stream.", ex, Logger.GenerateFullContextStackTrace());

                // return empty byte array
                return new byte[] { };
            }
        }

        /// <summary>
        /// Generates a file hash using the specified hash algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm to use.</param>
        /// <param name="stream">The full path to the file on disk.</param>
        /// <returns>A byte array containing the hash</returns>
        public byte[] GenerateFileHash(HashAlgorithmName hashAlgorithm, FileStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                using (var hasher = HashAlgorithm.Create(hashAlgorithm.Name))
                {
                    return hasher.ComputeHash(stream);
                }
            }
            catch (Exception ex)
            {
                // log the error
                Logger.WriteTraceError("Failed to generate a hash from stream.", ex, Logger.GenerateFullContextStackTrace());

                // return empty byte array
                return new byte[] { };
            }
        }

        /// <summary>
        /// Converts an array of bytes into a string seperated with dashes (-)
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string ConvertHashByteArrayToString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b + "-");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// Runs a comparison to see if two hashes are the same.
        /// </summary>
        /// <param name="hash1"></param>
        /// <param name="hash2"></param>
        /// <returns></returns>
        public bool CheckTwoByteHashesAreTheSame(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the default hash algorithm used for a specified priority type.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public HashAlgorithmName GetDefaultHashAlgorithm(FileBackupPriority priority)
        {
            if (priority == FileBackupPriority.Low)
            {
                return HashAlgorithmName.SHA1;
            }
            else if (priority == FileBackupPriority.Medium)
            {
                return HashAlgorithmName.SHA256;
            }
            else if (priority == FileBackupPriority.High)
            {
                return HashAlgorithmName.SHA512;
            }
            else
            {
                throw new NotImplementedException("Unexpected file backup priority type: " + priority);
            }
        }
    }
}
