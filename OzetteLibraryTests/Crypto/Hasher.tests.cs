using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OzetteLibraryTests.Crypto
{
    [TestClass()]
    public class Hasher
    {
        [TestMethod()]
        public void HashBytesToString()
        {
            OzetteLibrary.Crypto.Hasher h = new OzetteLibrary.Crypto.Hasher();

            byte[] bytes = { 39, 16, 25, 68, 128, 64, 55, 27 };

            Assert.AreEqual("39-16-25-68-128-64-55-27", h.HashBytesToString(bytes));
        }

        [TestMethod()]
        public void Generate20ByteSmallFileHash()
        {
            OzetteLibrary.Crypto.Hasher h = new OzetteLibrary.Crypto.Hasher();

            byte[] actualHash = h.Generate20ByteFileHash(".\\TestFiles\\SmallFile.txt");
            byte[] expectedHash = { 39, 16, 25, 68, 128, 64, 131, 150, 216, 25,
                                    111, 227, 135, 18, 122, 14, 163, 85, 226, 30 };

            Assert.AreEqual(expectedHash.Length, actualHash.Length);

            for (int i = 0; i < expectedHash.Length; i++)
            {
                Assert.AreEqual(expectedHash[i], actualHash[i], 
                    string.Format("Byte index {0} is incorrect. Encountered: {1}. Expected {2}", 
                    i, h.HashBytesToString(actualHash), h.HashBytesToString(expectedHash)));
            }
        }

        [TestMethod()]
        public void Generate32ByteSmallFileHash()
        {
            OzetteLibrary.Crypto.Hasher h = new OzetteLibrary.Crypto.Hasher();

            byte[] actualHash = h.Generate32ByteFileHash(".\\TestFiles\\SmallFile.txt");
            byte[] expectedHash = { 105, 105, 114, 199, 90, 53, 52, 53, 173, 45,
                                    254, 222, 255, 128, 156, 96, 21, 79, 196, 31,
                                    4, 129, 237, 45, 93, 69, 246, 90, 61, 209, 80, 64 };

            Assert.AreEqual(expectedHash.Length, actualHash.Length);

            for (int i = 0; i < expectedHash.Length; i++)
            {
                Assert.AreEqual(expectedHash[i], actualHash[i], 
                    string.Format("Byte index {0} is incorrect. Encountered: {1}. Expected {2}", 
                    i, h.HashBytesToString(actualHash), h.HashBytesToString(expectedHash)));
            }
        }

        [TestMethod()]
        public void Generate64ByteSmallFileHash()
        {
            OzetteLibrary.Crypto.Hasher h = new OzetteLibrary.Crypto.Hasher();

            byte[] actualHash = h.Generate64ByteFileHash(".\\TestFiles\\SmallFile.txt");
            byte[] expectedHash = { 30, 175, 186, 169, 203, 212, 173, 146, 227, 20, 65,
                                    15, 115, 42, 115, 68, 95, 1, 236, 65, 180, 175, 251,
                                    135, 40, 244, 205, 140, 242, 222, 6, 7, 140, 4, 179,
                                    223, 179, 50, 151, 56, 251, 15, 145, 127, 204, 104, 72,
                                    5, 201, 129, 225, 249, 132, 172, 224, 172, 225, 206,
                                    164, 213, 166, 106, 49, 170 };

            Assert.AreEqual(expectedHash.Length, actualHash.Length);

            for (int i = 0; i < expectedHash.Length; i++)
            {
                Assert.AreEqual(expectedHash[i], actualHash[i], 
                    string.Format("Byte index {0} is incorrect. Encountered: {1}. Expected {2}", 
                    i, h.HashBytesToString(actualHash), h.HashBytesToString(expectedHash)));
            }
        }
    }
}
