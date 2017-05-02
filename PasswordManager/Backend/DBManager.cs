using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Backend
{
    public class DBManager
    {
        private SQLiteConnection _db;
        private string _password = "";
        private string _path = "";
        private string _workingPath = "";

        public void CreateDB(string path, string password)
        {
            // Ensure database does not already exist
            if (File.Exists(path)) return;

            string working = path + ".tmp";

            // Create database file
            SQLiteConnection.CreateFile(working);

            // Encrypt database file
            EncryptFile(working, path, password);

            // Delete unencrypted file
            File.Delete(working);
        }

        /// <summary>
        /// Open the database at the given inputPath, using the given password
        /// </summary>
        /// <param name="path">
        /// Path to database
        /// </param>
        /// <param name="password">
        /// Password to decrypt database
        /// </param>
        public void OpenDB(string path, string password)
        {
            // Check if database exists
            if (!File.Exists(path))
            {
                // Create it if not
                CreateDB(path, password);
            }

            // Store database file info
            _path = path;
            _password = password;
            _workingPath = _path + ".tmp";

            // Decrypt database to a working copy if the password is correct
            DecryptFile(path, _workingPath, password);

            // Open database connection
            _db = new SQLiteConnection(_workingPath);
        }

        public void CloseDB()
        {
            if (_db == null) return;

            // Close database connection
            _db.Close();
            // Wait for database connection to fully terminate
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Encrypt database over the original using original password
            EncryptFile(_workingPath, _path, _password);

            // Delete the working copy
            File.Delete(_workingPath);

            // Clear database file info
            _path = "";
            _workingPath = "";
            _password = "";
        }

        private static void EncryptFile(string inputPath, string outputPath, string password)
        {
            // Configure AES
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.Key = Encoding.ASCII.GetBytes(password);
            crypto.IV = Encoding.ASCII.GetBytes(password);

            // Create encryptor
            ICryptoTransform encryptor = crypto.CreateEncryptor();

            // Stream for encrypted data
            FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            // Stream for encrypting data to disk
            CryptoStream encryptStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);

            // Read contents of file into memory
            byte[] fileContents = File.ReadAllBytes(inputPath);

            // Write encrypted data to file
            encryptStream.Write(fileContents, 0, fileContents.Length);
        }

        private static void DecryptFile(string inputPath, string outputPath, string password)
        {
            // Configure AES
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.Key = Encoding.ASCII.GetBytes(password);
            crypto.IV = Encoding.ASCII.GetBytes(password);

            // Decrypt the file
            ICryptoTransform decryptor = crypto.CreateDecryptor();

            // Stream for encrypted data
            FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
            // Stream for decrypting data to memory
            CryptoStream decryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
            // Stream for writing decrypted data to disk
            StreamWriter decryptedStream = new StreamWriter(outputPath);

            // Write decrypted data to disk
            decryptedStream.Write(new StreamReader(decryptStream).ReadToEnd());
            decryptedStream.Flush();
            decryptedStream.Close();
        }
    }
}