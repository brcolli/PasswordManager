using System;
using System.Text;
using NUnit.Framework;
using PasswordManager.Backend;

namespace PasswordManagerTests.Backend
{
    [TestFixture()]
    public class AESTests
    {
        [Test]
        public void AESRoundTripTest()
        {
            string orig = "I like to eat pizza all day long!";
            string password = "Super secure";
            string hash = PasswordHash.HashPassword(password);
            byte[] key = PasswordHash.GetDecryptedKey(password, hash);
            byte[] encrypted = AES.AESEncrypt(Encoding.ASCII.GetBytes(orig), key, key, 16);
            string decrypted = Encoding.ASCII.GetString(AES.AESDecrypt(encrypted, key, key, 16));
            Assert.AreEqual(orig, decrypted);
        }
    }
}