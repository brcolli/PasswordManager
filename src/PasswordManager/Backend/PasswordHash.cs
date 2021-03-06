﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Backend
{

    /// <summary>
    /// Inspired by https://security.stackexchange.com/questions/38828/how-can-i-securely-convert-a-string-password-to-a-key-used-in-aes
    /// with some parts based on code found at https://cmatskas.com/-net-password-hashing-using-pbkdf2/
    /// It uses PBKDF2 to hash passwords and validate them
    /// </summary>
    public class PasswordHash
    {
        public const int IVByteSize = 16;
        public const int SaltByteSize = 8;
        public const int HashByteSize = 32; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        public const int Pbkdf2Iterations = 1000;
        public const int IterationIndex = 0;
        public const int EncryptedIndex = 1;
        public const int K3Index = 2;
        public const int SaltIndex = 3;
        public const int IVIndex = 4;

        /// <summary>
        /// Generate AES encrypted key and required decryption info which can be safely
        /// stored on disk and requires password to decrypt
        /// </summary>
        /// <param name="password">
        /// Password to encrypt key with
        /// </param>
        /// <param name="k1Init">
        /// Value to use for k1, if null will generate random
        /// </param>
        /// <returns>
        /// String which can be fed into GetDecryptedKey along with password to get a
        /// decrypted key for doing AES encoding/decoding
        /// </returns>
        public static string HashPassword(string password, byte[] k1Init=null)
        {
            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
            // Get IVByteSize * 8 bit key
            byte[] k1 = k1Init ?? new byte[IVByteSize];
            cryptoProvider.GetBytes(k1);
            // Get IVByteSize * 8 bit IV
            byte[] iv = new byte[IVByteSize];
            cryptoProvider.GetBytes(iv);
            // Get SaltByteSize * 8 bit salt
            byte[] salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);

            // Get HashByteSize * 8 bit key using PBKDF2
            byte[] hash = GetPbkdf2Bytes(password, salt, Pbkdf2Iterations, HashByteSize);
            // Split into two (HashByteSize/2)*8-bit keys
            byte[] k2 = hash.Take(HashByteSize/2).ToArray();
            byte[] k3 = hash.Skip(HashByteSize/2).Take(HashByteSize/2).ToArray();

            byte[] encrypted = AES.AESEncrypt(k1, k2, iv, IVByteSize);

            return Pbkdf2Iterations + ":" +
                   Convert.ToBase64String(encrypted) + ":" +
                   Convert.ToBase64String(k3) + ":" +
                   Convert.ToBase64String(salt) + ":" +
                   Convert.ToBase64String(iv);
        }

        /// <summary>
        /// Decrypt the hashstring returned from HashPassword using the original password to
        /// get a secret key which can be used for AES encoding/decoding
        /// </summary>
        /// <param name="password">
        /// Password to decrypt hashstring with
        /// </param>
        /// <param name="hashString">
        /// Hashstring returned from HashPassword
        /// </param>
        /// <returns>
        /// Secret key which can be used for AES encoding/decoding and SHOULD NOT BE SAVED
        /// </returns>
        public static byte[] GetDecryptedKey(string password, string hashString)
        {
            char[] delimiter = { ':' };
            string[] split = hashString.Split(delimiter);
            int iterations = Int32.Parse(split[IterationIndex]);
            byte[] salt = Convert.FromBase64String(split[SaltIndex]);
            byte[] iv = Convert.FromBase64String(split[IVIndex]);
            byte[] k3 = Convert.FromBase64String(split[K3Index]);

            // Generate k2 and k3 using provided information
            // Get HashByteSize * 8 bit key using PBKDF2
            byte[] hash = GetPbkdf2Bytes(password, salt, iterations, HashByteSize);
            // Split into two (HashByteSize/2)*8-bit keys
            byte[] k2 = hash.Take(HashByteSize/2).ToArray();
            byte[] k3Generated = hash.Skip(HashByteSize/2).Take(HashByteSize/2).ToArray();

            // Verify password
            if (!SlowEquals(k3, k3Generated))
            {
                // Password is incorrect
                return null;
            }

            return AES.AESDecrypt(k3Generated, k2, iv, IVByteSize);
        }

        /// <summary>
        /// Slow equivalence check to prevent brute-forcing
        /// </summary>
        /// <param name="a">
        /// Value to check equivalence with b
        /// </param>
        /// <param name="b">
        /// Value to check equivalence with a
        /// </param>
        /// <returns>
        /// Whether a and b are equal
        /// </returns>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
            
        /// <summary>
        /// Use PBKDF2 to generate pseudo-random bytes
        /// </summary>
        /// <param name="password">
        /// Password for PBKDF2
        /// </param>
        /// <param name="salt">
        /// Salt for PBKDF2
        /// </param>
        /// <param name="iterations">
        /// Number of iterations to use for PBKDF2
        /// </param>
        /// <param name="outputBytes">
        /// Number of bytes to generate
        /// </param>
        /// <returns>
        /// OutputBytes bytes generated using PBKDF2
        /// </returns>
        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}