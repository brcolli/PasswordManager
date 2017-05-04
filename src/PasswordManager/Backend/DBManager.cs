using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Backend
{
    /// <summary>
    /// Wrapper around encrypted SQLite database. Manages databases and encrypts/decrypts
    /// to and from a file on close/open respectively
    /// </summary>
    public class DBManager
    {
        private static DBManager _instance;
        private static string _path = "Databases/";
        private static string _dbExt = ".db";
        private static string _hashExt = ".key";
        private static readonly Encoding _hashEncoding = Encoding.UTF8;
        private static int _keySize = 16;

        private Database _db;
        private string _password = "";
        private string _dbPath = "";
        private string _hashPath = "";
        private string _workingPath = "";

        /// <summary>
        /// Get DBManager Singleton
        /// </summary>
        public static DBManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBManager();
                }
                return _instance;
            }
        }

        private DBManager()
        {
            // Create database storage directory
            Directory.CreateDirectory(_path);
        }

        public static void OnExit(object sender, EventArgs e)
        {
            Instance.CloseDB();
        }


        /// <summary>
        /// Check if the database with the given name exists
        /// </summary>
        /// <param name="name">
        /// Name of database to check for
        /// </param>
        /// <returns></returns>
        public bool DBExists(string name)
        {
            // TODO: This doesn't seem to be getting called which leaves an unencrypted database behind on close. This is bad!!!
            string path = _path + name + _dbExt;

            return File.Exists(path);
        }

        /// <summary>
        /// Create a database with the given name, and encrypt it with the given password.
        /// Saves the database and associated hashstring to the disk
        /// </summary>
        /// <param name="name">
        /// Name of database to create
        /// </param>
        /// <param name="password">
        /// Password for the database
        /// </param>
        /// <returns>
        /// Whether database was created successfully
        /// </returns>
        public bool CreateDB(string name, string password)
        {
            string dbPath = _path + name + _dbExt;
            string hashPath = _path + name + _hashExt;

            // Ensure database does not already exist
            if (File.Exists(dbPath)) return false;

            string working = dbPath + ".tmp";

            // Create password hash file
            string hashed = PasswordHash.HashPassword(password);
            File.WriteAllText(hashPath, hashed, _hashEncoding);

            byte[] key = PasswordHash.GetDecryptedKey(password, hashed);

            if (key == null) return false;

            // Create database file
            SQLiteConnection.CreateFile(working);

            // Initialize the database (necessary to make it not just 0s)
            _db = new Database(working);

            // Set up the database
            CreateAccountsTable();

            // Close the database
            _db.Close();

            // Encrypt database file
            byte[] workingData = File.ReadAllBytes(working);
            byte[] encryptedData = AES.AESEncrypt(workingData, key, key, _keySize);

            // Write encrypt data to disk
            File.WriteAllBytes(dbPath, encryptedData);
//            EncryptFile(working, dbPath, key);

            // Delete unencrypted file
            File.Delete(working);

            return true;
        }

        /// <summary>
        /// Open the database at the given inputPath, using the given password
        /// </summary>
        /// <param name="name">
        /// Path to database
        /// </param>
        /// <param name="password">
        /// Password to decrypt database
        /// </param>
        /// <returns>
        /// 1 if successful, 0 if password incorrect, 2 if no hash file found, 3 if couldn't open DB, and 4 if it couldn't create a DB and one was not found
        /// </returns>
        public int OpenDB(string name, string password)
        {
            string dbPath = _path + name + _dbExt;
            string hashPath = _path + name + _hashExt;

            // Ensure no open database and close if there is one
            if (_db != null) CloseDB();

            // Check if database exists
            if (!File.Exists(dbPath))
            {
                // Create it if not
                if (!CreateDB(name, password))
                {
                    // We should never get here
                    return 4;
                }
            }
            if (!File.Exists(hashPath))
            {
                // No hash file found, database can't be decrypted
                return 2;
            }

            // Store database file info
            _dbPath = dbPath;
            _hashPath = hashPath;
            _password = password;
            _workingPath = _dbPath + ".tmp";

            // Get decrypted key
            string hashed = File.ReadAllText(hashPath, _hashEncoding);
            byte[] key = PasswordHash.GetDecryptedKey(password, hashed);
            // Ensure password is valid
            if (key == null)
            {
                // Password incorrect
                return 0;
            }

            // Decrypt database file
            byte[] encryptedData = File.ReadAllBytes(dbPath);
            byte[] decryptedData = AES.AESDecrypt(encryptedData, key, key, _keySize);

            // Write decrypted data to disk
            File.WriteAllBytes(_workingPath, decryptedData);

            // Decrypt database to a working copy if the password is correct
//            DecryptFile(dbPath, _workingPath, key);

            // Open database
            _db = new Database(_workingPath);

            // Return 3 if db null, 1 if ok
            return _db == null ? 3 : 1;
        }

        /// <summary>
        /// Close any open database and encrypt it
        /// </summary>
        /// <returns>
        /// Whether a database was closed
        /// </returns>
        public bool CloseDB()
        {
            // Store local database file info
            string dbPath = _dbPath;
            string working = _workingPath;
            string password = _password;
            string hashPath = _hashPath;

            // Clear database file info
            _dbPath = "";
            _hashPath = "";
            _workingPath = "";
            _password = "";

            if (_db == null) return false;

            // Close the database
            _db.Close();
            _db = null;

            // Get decrypted key
            string hashed = File.ReadAllText(hashPath, _hashEncoding);
            byte[] key = PasswordHash.GetDecryptedKey(password, hashed);

            // Encrypt database over the original using origin password hash
            // Encrypt database file
            byte[] workingData = File.ReadAllBytes(working);
            byte[] encryptedData = AES.AESEncrypt(workingData, key, key, _keySize);

            // Write encrypt data to disk
            File.WriteAllBytes(dbPath, encryptedData);
//            EncryptFile(working, path, key);

            // Delete the working copy
            File.Delete(working);

            return true;
        }

        /// <summary>
        /// Check if the given password is correct
        /// </summary>
        /// <param name="password">
        /// Candidate password for database
        /// </param>
        /// <returns>
        /// 1 if correct, 0 if password incorrect, and 2 if no hash file found
        /// </returns>
        public int ValidatePassword(string password)
        {
            if (!File.Exists(_hashPath))
            {
                // No hash file found, database can't be decrypted
                return 2;
            }

            // Get decrypted key
            string hashed = File.ReadAllText(_hashPath, _hashEncoding);
            byte[] key = PasswordHash.GetDecryptedKey(password, hashed);
            // Ensure password is valid
            if (key == null)
            {
                // Password incorrect
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// Change the database password to the given password
        /// </summary>
        /// <param name="password">
        /// Original password for database
        /// </param>
        /// <param name="newPassword">
        /// Password to replace original password with
        /// </param>
        /// <returns>
        /// 1 if successful, 0 if password incorrect, and 2 if no database loaded
        /// </returns>
        public int ChangePassword(string password, string newPassword)
        {
            if (_db == null) return 2;

            // Get decrypted key
            string hashed = File.ReadAllText(_hashPath, _hashEncoding);
            byte[] key = PasswordHash.GetDecryptedKey(password, hashed);

            // Verify password is correct
            if (key == null) return 0;

            // Use key to encrypt password
            string newHashed = PasswordHash.HashPassword(newPassword, key);
            File.WriteAllText(_hashPath, newHashed, _hashEncoding);

            // Store new password
            _password = newPassword;

            return 1;
        }

        private void CreateAccountsTable()
        {
            string cmd = "CREATE TABLE IF NOT EXISTS Accounts (`ID`	                  TEXT NOT NULL UNIQUE, \n" +
                                                              "`Password`             TEXT NOT NULL, \n" +
                                                              "PRIMARY KEY(ID))";
            _db.ExecuteNonQuery(cmd);
        }

        public bool ContainsAccount(string id)
        {
            List<string> parameters = new List<string> {id};
            // Check if ID is in DB
            return (long)_db.ExecuteScalar("SELECT Count(*) FROM Accounts WHERE ID = @param1", parameters) == 1;
        }

        public void AddAccount(string id, string password)
        {
            List<string> parameters = new List<string>
            {
                id,
                password
            };
            _db.ExecuteNonQuery("INSERT INTO Accounts (" +
                            "ID, " +
                            "Password) " +
                            "VALUES (" + 
                            "@param1, @param2" + 
                            ")", parameters);
        }

        public void UpdateAccount(string id, string password)
        {
            List<string> parameters = new List<string>
            {
                id,
                password
            };
            _db.ExecuteNonQuery("UPDATE Accounts SET " +
                            "Password = @param2 " +
                            "WHERE ID = @param1", parameters);
        }

        /// <summary>
        /// Get data for account with given ID from database
        /// </summary>
        /// <param name="id">
        /// ID of account to lookup
        /// </param>
        /// <returns>
        /// Dictionary of account data values
        /// </returns>
        public Dictionary<string, string> GetAccount(string id)
        {
            List<string> parameters = new List<string> {id};
            Dictionary<string, string> data = new Dictionary<string, string>();
            // Find account
            using (SQLiteDataReader reader = _db.ExecuteReader("SELECT * FROM Accounts WHERE ID = @param1", parameters))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // Extract data
                        data["Password"] = (string) reader["Password"];
                    }
                }
                reader.Close();
            }
            return data;
        }

        /// <summary>
        /// Remove the account with the given ID from the database
        /// </summary>
        /// <param name="id">
        /// ID of account to remove
        /// </param>
        /// <returns>
        /// Whether database contained account
        /// </returns>
        public bool RemoveAccount(string id)
        {
            // Check if database contains account
            if (!ContainsAccount(id)) return false;
            // Delete account
            List<string> parameters = new List<string> {id};
            _db.ExecuteNonQuery("DELETE FROM Accounts WHERE ID = @param1", parameters);
            return true;
        }

//        /// <summary>
//        /// Encrypt the given input file using the given AES key to the given output file
//        /// </summary>
//        /// <param name="inputPath">
//        /// File to encrypt
//        /// </param>
//        /// <param name="outputPath">
//        /// File to encrypt to
//        /// </param>
//        /// <param name="key">
//        /// Key to use for AES encryption
//        /// </param>
//        public static void EncryptFile(string inputPath, string outputPath, byte[] key)
//        {
//            // TODO: Either this or decrypt is garbling the file. Doesn't happen with empty database
//            // Configure AES
//            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
//            crypto.KeySize = 128;
//            crypto.Padding = PaddingMode.None;
//            crypto.Key = key.Take(16).ToArray();
//            crypto.IV = key.Skip(0).Take(16).ToArray();
//
//            // Create encryptor
//            ICryptoTransform encryptor = crypto.CreateEncryptor();
//
//            // Stream for encrypted data
//            using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
//            {
//                // Stream for encrypting data to disk
//                using (CryptoStream encryptStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
//                {
//                    // Read contents of file into memory
//                    byte[] fileContents = File.ReadAllBytes(inputPath);
//
//                    // Write encrypted data to file
//                    encryptStream.Write(fileContents, 0, fileContents.Length);
//                }
//            }
//        }
//
//        /// <summary>
//        /// Decrypt the given input file using the given AES key to the given output file
//        /// </summary>
//        /// <param name="inputPath">
//        /// File to decrypt
//        /// </param>
//        /// <param name="outputPath">
//        /// File to decrypt to
//        /// </param>
//        /// <param name="key">
//        /// Key to use for AES decryption
//        /// </param>
//        public static void DecryptFile(string inputPath, string outputPath, byte[] key)
//        {
//            // Configure AES
//            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
//            crypto.KeySize = 128;
//            crypto.Padding = PaddingMode.None;
//            crypto.Key = key.Take(16).ToArray();
//            crypto.IV = key.Skip(0).Take(16).ToArray();
//
//            // Decrypt the file
//            ICryptoTransform decryptor = crypto.CreateDecryptor();
//
//            // Stream for encrypted data
//            using (FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
//            {
//                // Stream for decrypting data to memory
//                using (CryptoStream decryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
//                {
//                    // Stream for writing decrypted data to disk
//                    using (StreamWriter decryptedStream = new StreamWriter(outputPath))
//                    {
//                        // Write decrypted data to disk
//                        decryptedStream.Write(new StreamReader(decryptStream).ReadToEnd());
//                    }
//                }
//            }
//        }
    }
}