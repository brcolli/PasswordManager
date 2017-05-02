using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace PasswordManager.Backend
{
    public class Database
    {
        private SQLiteConnection _db = null;

        public Database() { }

        /// <summary>
        /// Create a Database and open the SQLite database at the given path
        /// </summary>
        /// <param name="path">
        /// Path to database to open
        /// </param>
        public Database(string path)
        {
            Open(path);
        }

        /// <summary>
        /// Open the SQLite database at the given path
        /// </summary>
        /// <param name="path">
        /// Path to database to open
        /// </param>
        public void Open(string path)
        {
            // Ensure database is closed
            if (_db != null)
            {
                Close();
            }
            
            SQLiteConnectionStringBuilder stringBuilder = new SQLiteConnectionStringBuilder()
            {
                DataSource = path,
            };
            _db = new SQLiteConnection(stringBuilder.ConnectionString);
            _db.Open();
        }

        /// <summary>
        /// Close the open database, if any
        /// </summary>
        public void Close()
        {
            if (_db == null) return;

            // Close database connection
            _db.Close();
            _db = null;

            // Wait for database connection to fully terminate
            GC.Collect();
            GC.WaitForPendingFinalizers();
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
    }
}