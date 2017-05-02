using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Backend
{
    /// <summary>
    /// Utility class for AES encryption and decryption
    /// </summary>
    public class AES
    {
        /// <summary>
        /// Encrypt the given data using the given key and iv
        /// </summary>
        /// <param name="data">
        /// Data to encrypt using AES
        /// </param>
        /// <param name="key">
        /// Key to encrypt data with
        /// </param>
        /// <param name="iv">
        /// IV to encrypt data with
        /// </param>
        /// <param name="keySize">
        /// Size of key in bytes to use for AES encryption
        /// </param>
        /// <returns>
        /// Encrypted data
        /// </returns>
        public static byte[] AESEncrypt(byte[] data, byte[] key, byte[] iv, int keySize)
        {
            // Configure AES
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = keySize * 8;
            crypto.Padding = PaddingMode.ISO10126;
            crypto.Key = key;
            crypto.IV = iv;

            // Create encryptor
            ICryptoTransform encryptor = crypto.CreateEncryptor();

            byte[] encrypted;

            // Stream for encrypted data
            using (MemoryStream inputStream = new MemoryStream(data))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    // Stream for encrypting data
                    using (CryptoStream encryptStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                    {
                        // Write encrypted data to stream
                        byte[] buffer = new byte[1024];
                        int read = inputStream.Read(buffer, 0, buffer.Length);
                        while (read > 0)
                        {
                            encryptStream.Write(buffer, 0, read);
                            read = inputStream.Read(buffer, 0, buffer.Length);
                        }

//                            encryptStream.Write(data, 0, (int)encryptStream.Length);
                        encryptStream.FlushFinalBlock();
                    }
                }
                encrypted = inputStream.ToArray();
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypt given data using given key and iv
        /// </summary>
        /// <param name="data">
        /// Data to decrypt
        /// </param>
        /// <param name="key">
        /// Key to decrypt with
        /// </param>
        /// <param name="iv">
        /// IV to decrypt with
        /// </param>
        /// <param name="keySize">
        /// Size of key in bytes to use for AES decryption
        /// </param>
        /// <returns>
        /// Decrypted data
        /// </returns>
        public static byte[] AESDecrypt(byte[] data, byte[] key, byte[] iv, int keySize)
        {
            // Configure AES
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = keySize * 8;
            crypto.Padding = PaddingMode.ISO10126;
            crypto.Key = key;
            crypto.IV = iv;

            // Create decryptor
            ICryptoTransform decryptor = crypto.CreateDecryptor();

            byte[] decrypted = new byte[keySize];

            // Stream for encrypted data
            using (MemoryStream inputStream = new MemoryStream(data))
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    // Stream for encrypting data
                    using (CryptoStream decryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        // Write decrypted data to stream
                        byte[] buffer = new byte[1024];
                        int read = inputStream.Read(buffer, 0, buffer.Length);
                        while (read > 0)
                        {
                            outputStream.Write(buffer, 0, read);
                            read = inputStream.Read(buffer, 0, buffer.Length);
                        }
                        decryptStream.Flush();
                        // Read decrypted data from stream
                        //                    decryptStream.Read(decrypted, 0, (int)decryptStream.Length);
                    }
                    decrypted = outputStream.ToArray();
                }
            }

            return decrypted;  //Encoding.UTF8.GetBytes(decrypted);
        }
    }
}