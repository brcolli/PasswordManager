using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Backend
{
    public class DBManager
    {
        private static DBManager _instance;
        private static string _path = "Databases/";
        private static string _dbExt = ".db";
        private static string _hashExt = ".key";
        private static readonly Encoding _hashEncoding = Encoding.UTF8;

        private SQLiteConnection _db;
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

            // Encrypt database file
            EncryptFile(working, dbPath, key);

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

            // Decrypt database to a working copy if the password is correct
            DecryptFile(dbPath, _workingPath, key);

            // Open database connection
            SQLiteConnectionStringBuilder stringBuilder = new SQLiteConnectionStringBuilder()
            {
                DataSource = _workingPath,
            };
            _db = new SQLiteConnection(stringBuilder.ConnectionString);
            _db.Open();

            // Ensure database is set up
            CreateAccountsTable();

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
            string path = _dbPath;
            string working = _workingPath;
            string password = _password;
            string hashPath = _hashPath;

            // Clear database file info
            _dbPath = "";
            _hashPath = "";
            _workingPath = "";
            _password = "";

            if (_db == null) return false;

            // Close database connection
            _db.Close();
            _db = null;
            // Wait for database connection to fully terminate
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Get decrypted key
            string hashed = File.ReadAllText(hashPath, _hashEncoding);
            byte[] key = PasswordHash.GetDecryptedKey(password, hashed);

            // Encrypt database over the original using origin password hash
            EncryptFile(working, path, key);

            // Delete the working copy
            File.Delete(working);

            return true;
        }

        /// <summary>
        /// Execute given command on SQLite database
        /// </summary>
        /// <param name="commandString">
        /// Command to pass to database
        /// </param>
        /// <param name="parameters">
        /// List of variables to pass into @param#'s in commandString
        /// </param>
        /// <param name="command">
        /// SQLiteCommand to execute non-query on
        /// </param>
        public void ExecuteNonQuery(string commandString, List<string> parameters=null, SQLiteCommand command=null)
	    {
            int index = 1;
            if (command == null)
            {
                // Make a new command
                using (command = _db.CreateCommand())
                {
                    if (parameters != null)
                    {
                        foreach (string param in parameters)
                        {
                            command.Parameters.Add(new SQLiteParameter("@param" + index, param));
                            index++;
                        }
                    }
                    command.CommandText = commandString;
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                if (parameters != null)
                {
                    foreach (string param in parameters)
                    {
                        command.Parameters.Add(new SQLiteParameter("@param" + index, param));
                        index++;
                    }
                }
                command.CommandText = commandString;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute given query on SQLite database and return results
        /// </summary>
        /// <param name="commandString">
        /// Query to pass to database
        /// </param>
        /// <param name="parameters">
        /// List of variables to pass into @param#'s in commandString
        /// </param>
        /// <returns>
        /// SQLiteDataReader containing results from query
        /// </returns>
	    public SQLiteDataReader ExecuteReader(string commandString, List<string> parameters=null)
	    {
            using (SQLiteCommand command = new SQLiteCommand(commandString, _db))
            {
                int index = 1;
                if (parameters != null)
                {
                    foreach (string param in parameters)
                    {
                        command.Parameters.Add(new SQLiteParameter("@param" + index, param));
                        index++;
                    }
                }
                command.CommandText = commandString;
                return command.ExecuteReader();
            }
	    }

        /// <summary>
        /// Execute given query on SQLite database and return first column of first row
        /// of query results
        /// </summary>
        /// <param name="commandString">
        /// Command to pass to database
        /// </param>
        /// <param name="parameters">
        /// List of variables to pass into @param#'s in commandString
        /// </param>
        /// <returns>
        /// First column of first row of query results
        /// </returns>
	    public object ExecuteScalar(string commandString, List<string> parameters=null)
	    {
            using (SQLiteCommand command = new SQLiteCommand(commandString, _db))
            {
                int index = 1;
                if (parameters != null)
                {
                    foreach (string param in parameters)
                    {
                        command.Parameters.Add(new SQLiteParameter("@param" + index, param));
                        index++;
                    }
                }
                command.CommandText = commandString;
                return command.ExecuteScalar();
            }
	    }

        /// <summary>
        /// Get the number of rows in the given table
        /// </summary>
        /// <param name="table">
        /// Name of table to get the number of rows of
        /// </param>
        /// <returns>
        /// Number of rows in the given table
        /// </returns>
	    public long GetRowCount(string table)
	    {
	        return (long)ExecuteScalar("SELECT Count(*) FROM " + table);
	    }

        public SQLiteDataReader GetLastRow(string table)
        {
            return ExecuteReader("SELECT * FROM " + table + " ORDER BY rowid DESC LIMIT 1");
        }

        /// <summary>
        /// Check whether the given column of the reader is null
        /// </summary>
        /// <param name="reader">
        /// Reader to check for null on
        /// </param>
        /// <param name="column">
        /// Column to check for null on
        /// </param>
        /// <returns>
        /// Whether the given column of the reader is null
        /// </returns>
        public bool IsColumnNull(SQLiteDataReader reader, string column)
        {
            return reader.IsDBNull(reader.GetOrdinal(column));
        }

        public bool ContainsAccount(string id)
        {
            List<string> parameters = new List<string> {id};
            // Check if ID is in DB
            // TODO: This is null for some reason
            return (long)ExecuteScalar("SELECT Count(*) FROM Accounts WHERE ID = @param1", parameters) == 1;
        }

        public void AddAccount(string id, string password)
        {
            List<string> parameters = new List<string>
            {
                id,
                password
            };
            ExecuteNonQuery("INSERT INTO Accounts (" +
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
            ExecuteNonQuery("UPDATE Accounts SET " +
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
            using (SQLiteDataReader reader = ExecuteReader("SELECT * FROM Accounts WHERE ID = @param1", parameters))
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
            ExecuteNonQuery("DELETE FROM Accounts WHERE ID = @param1", parameters);
            return true;
        }

        private void CreateAccountsTable()
        {
            string cmd = "CREATE TABLE IF NOT EXISTS Accounts (`ID`	                  TEXT NOT NULL UNIQUE, \n" +
                                                              "`Password`             TEXT NOT NULL, \n" +
                                                              "PRIMARY KEY(ID))";
            ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Encrypt the given input file using the given AES key to the given output file
        /// </summary>
        /// <param name="inputPath">
        /// File to encrypt
        /// </param>
        /// <param name="outputPath">
        /// File to encrypt to
        /// </param>
        /// <param name="key">
        /// Key to use for AES encryption
        /// </param>
        private static void EncryptFile(string inputPath, string outputPath, byte[] key)
        {
            // TODO: Either this or decrypt is garbling the file. Doesn't happen with empty database
            // Configure AES
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = 128;
            crypto.Padding = PaddingMode.None;
            crypto.Key = key.Take(16).ToArray();
            crypto.IV = key.Skip(0).Take(16).ToArray();

            // Create encryptor
            ICryptoTransform encryptor = crypto.CreateEncryptor();

            // Stream for encrypted data
            using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                // Stream for encrypting data to disk
                using (CryptoStream encryptStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                {
                    // Read contents of file into memory
                    byte[] fileContents = File.ReadAllBytes(inputPath);

                    // Write encrypted data to file
                    encryptStream.Write(fileContents, 0, fileContents.Length);
                }
            }
        }

        /// <summary>
        /// Decrypt the given input file using the given AES key to the given output file
        /// </summary>
        /// <param name="inputPath">
        /// File to decrypt
        /// </param>
        /// <param name="outputPath">
        /// File to decrypt to
        /// </param>
        /// <param name="key">
        /// Key to use for AES decryption
        /// </param>
        private static void DecryptFile(string inputPath, string outputPath, byte[] key)
        {
            // Configure AES
            AesCryptoServiceProvider crypto = new AesCryptoServiceProvider();
            crypto.KeySize = 128;
            crypto.Padding = PaddingMode.None;
            crypto.Key = key.Take(16).ToArray();
            crypto.IV = key.Skip(0).Take(16).ToArray();

            // Decrypt the file
            ICryptoTransform decryptor = crypto.CreateDecryptor();

            // Stream for encrypted data
            using (FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            {
                // Stream for decrypting data to memory
                using (CryptoStream decryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                {
                    // Stream for writing decrypted data to disk
                    using (StreamWriter decryptedStream = new StreamWriter(outputPath))
                    {
                        // Write decrypted data to disk
                        decryptedStream.Write(new StreamReader(decryptStream).ReadToEnd());
                    }
                }
            }
        }
    }
}