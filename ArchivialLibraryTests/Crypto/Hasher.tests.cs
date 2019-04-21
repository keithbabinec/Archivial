using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArchivialLibrary.Logging.Mock;
using System;
using System.Security.Cryptography;

namespace OzetteLibraryTests.Crypto
{
    [TestClass]
    public class HasherTests
    {
        [TestMethod]
        public void HasherCanCorrectlyConvertHashBytesToString()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes = { 39, 16, 25, 68, 128, 64, 55, 27 };

            Assert.AreEqual("39-16-25-68-128-64-55-27", h.ConvertHashByteArrayToString(bytes));
        }

        [TestMethod]
        public void HasherCompareReturnsTrueForSameHash1()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 39, 16, 25, 68, 128, 64, 55, 27 };
            byte[] bytes2 = { 39, 16, 25, 68, 128, 64, 55, 27 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCompareReturnsTrueForSameHash2()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 39, 16, 25, 68, 128, 64, 55, 27 };
            byte[] bytes2 = { 39, 16, 25, 68, 128, 64, 55, 27 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(bytes2, bytes1));
        }

        [TestMethod]
        public void HasherCompareReturnsTrueForSameHash3()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 39, 16, 25, 68, 128, 64, 131, 150, 216, 25,
                                    111, 227, 135, 18, 122, 14, 163, 85, 226, 30 };
            byte[] bytes2 = { 39, 16, 25, 68, 128, 64, 131, 150, 216, 25,
                                    111, 227, 135, 18, 122, 14, 163, 85, 226, 30 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCompareReturnsTrueForSameHash4()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };
            byte[] bytes2 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCompareReturnsFalseForDifferentHashes1()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 235, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };
            byte[] bytes2 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };

            Assert.IsFalse(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCompareReturnsFalseForDifferentHashes2()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 208, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };
            byte[] bytes2 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };

            Assert.IsFalse(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCompareReturnsFalseForDifferentHashes3()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };
            byte[] bytes2 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136 };

            Assert.IsFalse(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCompareReturnsFalseForDifferentHashes4()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] bytes1 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };
            byte[] bytes2 = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 216 };

            Assert.IsFalse(h.CheckTwoByteHashesAreTheSame(bytes1, bytes2));
        }

        [TestMethod]
        public void HasherCanCorrectlyGenerate20ByteSmallFileHash()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.Generate20ByteFileHash(".\\TestFiles\\Hasher\\SmallFile.txt");
            byte[] expectedHash = { 39, 16, 25, 68, 128, 64, 131, 150, 216, 25,
                                    111, 227, 135, 18, 122, 14, 163, 85, 226, 30 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(actualHash, expectedHash));
        }

        [TestMethod]
        public void HasherCanCorrectlyGenerate32ByteSmallFileHash()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.Generate32ByteFileHash(".\\TestFiles\\Hasher\\SmallFile.txt");
            byte[] expectedHash = { 105, 105, 114, 199, 90, 53, 52, 53, 173, 45,
                                    254, 222, 255, 128, 156, 96, 21, 79, 196, 31,
                                    4, 129, 237, 45, 93, 69, 246, 90, 61, 209, 80, 64 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(actualHash, expectedHash));
        }

        [TestMethod]
        public void HasherCanCorrectlyGenerate64ByteSmallFileHash()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.Generate64ByteFileHash(".\\TestFiles\\Hasher\\SmallFile.txt");
            byte[] expectedHash = { 30, 175, 186, 169, 203, 212, 173, 146, 227, 20, 65,
                                    15, 115, 42, 115, 68, 95, 1, 236, 65, 180, 175, 251,
                                    135, 40, 244, 205, 140, 242, 222, 6, 7, 140, 4, 179,
                                    223, 179, 50, 151, 56, 251, 15, 145, 127, 204, 104, 72,
                                    5, 201, 129, 225, 249, 132, 172, 224, 172, 225, 206,
                                    164, 213, 166, 106, 49, 170 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(actualHash, expectedHash));
        }

        [TestMethod]
        public void HasherCanCorrectlyGenerate20ByteMediumFileHash()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.Generate20ByteFileHash(".\\TestFiles\\Hasher\\MediumFile.mp3");
            byte[] expectedHash = { 59, 46, 56, 72, 141, 74, 31, 120, 17, 24, 52,
                                    218, 140, 133, 179, 49, 83, 159, 149, 50 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(actualHash, expectedHash));
        }

        [TestMethod]
        public void HasherCanCorrectlyGenerate32ByteMediumFileHash()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.Generate32ByteFileHash(".\\TestFiles\\Hasher\\MediumFile.mp3");
            byte[] expectedHash = { 59, 223, 174, 134, 63, 70, 226, 183, 168, 86, 148,
                                    117, 40, 147, 139, 169, 231, 2, 41, 38, 8, 109, 55,
                                    221, 0, 10, 29, 173, 245, 93, 11, 59 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(actualHash, expectedHash));
        }

        [TestMethod]
        public void HasherCanCorrectlyGenerate64ByteMediumFileHash()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.Generate64ByteFileHash(".\\TestFiles\\Hasher\\MediumFile.mp3");
            byte[] expectedHash = { 234, 71, 228, 136, 179, 247, 148, 228, 21, 49, 132, 155,
                                    219, 239, 173, 165, 249, 35, 144, 199, 224, 127, 103,
                                    207, 211, 205, 191, 43, 207, 232, 154, 191, 238, 13, 128,
                                    221, 58, 116, 68, 175, 197, 121, 110, 243, 35, 53, 228,
                                    214, 206, 251, 250, 253, 35, 149, 27, 127, 233, 201, 36,
                                    219, 107, 45, 136, 215 };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(actualHash, expectedHash));
        }

        [TestMethod]
        public void HasherGenerateDefaultHashReturnsCorrectLength1()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.GenerateDefaultHash(".\\TestFiles\\Hasher\\SmallFile.txt", ArchivialLibrary.Files.FileBackupPriority.Low);

            Assert.AreEqual(20, actualHash.Length);
        }

        [TestMethod]
        public void HasherGenerateDefaultHashReturnsCorrectLength2()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.GenerateDefaultHash(".\\TestFiles\\Hasher\\SmallFile.txt", ArchivialLibrary.Files.FileBackupPriority.Medium);

            Assert.AreEqual(32, actualHash.Length);
        }

        [TestMethod]
        public void HasherGenerateDefaultHashReturnsCorrectLength3()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.GenerateDefaultHash(".\\TestFiles\\Hasher\\SmallFile.txt", ArchivialLibrary.Files.FileBackupPriority.High);

            Assert.AreEqual(64, actualHash.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void HasherGenerateDefaultHashThrowsOnInvalidBackupPriority()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] actualHash = h.GenerateDefaultHash(".\\TestFiles\\Hasher\\SmallFile.txt", 0);
        }

        [TestMethod]
        public void HasherGetDefaultHashAlgorithmReturnsCorrectAlgorithm1()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            HashAlgorithmName actual = h.GetDefaultHashAlgorithm(ArchivialLibrary.Files.FileBackupPriority.Low);

            Assert.AreEqual(HashAlgorithmName.SHA1, actual);
        }

        [TestMethod]
        public void HasherGetDefaultHashAlgorithmReturnsCorrectAlgorithm2()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            HashAlgorithmName actual = h.GetDefaultHashAlgorithm(ArchivialLibrary.Files.FileBackupPriority.Medium);

            Assert.AreEqual(HashAlgorithmName.SHA256, actual);
        }

        [TestMethod]
        public void HasherGetDefaultHashAlgorithmReturnsCorrectAlgorithm3()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            HashAlgorithmName actual = h.GetDefaultHashAlgorithm(ArchivialLibrary.Files.FileBackupPriority.High);

            Assert.AreEqual(HashAlgorithmName.SHA512, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void HasherGetDefaultHashAlgorithmThrowsOnInvalidBackupPriority()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            HashAlgorithmName actual = h.GetDefaultHashAlgorithm(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasherHashFileBlockFromByteArrayThrowsOnBadInput1()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());
            h.HashFileBlockFromByteArray(HashAlgorithmName.SHA256, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasherHashFileBlockFromByteArrayThrowsOnBadInput2()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());
            h.HashFileBlockFromByteArray(HashAlgorithmName.SHA256, new byte[0]);
        }

        [TestMethod]
        public void HasherHashFileBlockFromByteArrayCorrectlyHashesInput1()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());
            var result = h.HashFileBlockFromByteArray(
                HashAlgorithmName.SHA1,
                new byte[]
                {
                     123, 126, 219, 146, 255, 226,  17,  43, 110,  18, 215, 180, 113, 204, 233, 139,
                      70, 123, 173, 184,  10,  15,  97, 130,  74,  35, 247,   6, 175, 237, 224, 213,
                     148,   5,  56, 227,  19,  95, 192,  34, 202, 132,  25,  83, 238, 245,  36, 161,
                     175, 152,  72,  57, 253, 209, 189,  38, 190, 171,  12, 164,  38,  66,   1, 226
                });

            var expected = new byte[]
            {
                101, 135, 62, 112, 4, 11, 126, 120, 77, 230, 134, 32, 75, 23, 92, 51, 210, 30, 32, 69
            };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(result, expected));
        }

        [TestMethod]
        public void HasherHashFileBlockFromByteArrayCorrectlyHashesInput2()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());
            var result = h.HashFileBlockFromByteArray(
                HashAlgorithmName.SHA256,
                LargeByteStreamConstants.LargeByteStream);

            var expected = new byte[]
            {
                243,10,203,238,4,236,188,44,192,33,121,88,216,28,69,38,148,10,50,148,149,213,98,172,50,11,162,156,0,118,246,92
            };

            Assert.IsTrue(h.CheckTwoByteHashesAreTheSame(result, expected));
        }

        [TestMethod]
        public void HasherConvertHashByteArrayToBase64EncodedStringReturnsValidBase64String()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            var input = new byte[]
            {
                243,10,203,238,4,236,188,44,192,33,121,88,216,28,69,38,148,10,50,148,149,213,98,172,50,11,162,156,0,118,246,92
            };

            var result = h.ConvertHashByteArrayToBase64EncodedString(input);
            var expected = "8wrL7gTsvCzAIXlY2BxFJpQKMpSV1WKsMguinAB29lw=";

            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasherConvertHashByteArrayToBase64EncodedStringThrowsOnNullByteArray()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] input = null;

            h.ConvertHashByteArrayToBase64EncodedString(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasherConvertHashByteArrayToBase64EncodedStringThrowsOnEmptyByteArray()
        {
            ArchivialLibrary.Crypto.Hasher h = new ArchivialLibrary.Crypto.Hasher(new MockLogger());

            byte[] input = new byte[0];

            h.ConvertHashByteArrayToBase64EncodedString(input);
        }
    }
}
