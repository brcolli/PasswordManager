using System;
using System.Collections.Generic;
using System.Data.SQLite;
using PasswordManager.Backend;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// The page responsible for retrieving the passwords,
    /// editing the passwords, deleting, etc.
    /// </summary>

    class ManagementPage: Table
    {

        /// <summary>
        /// The management page
        /// </summary>
        /// <param name="gm">
        /// The manager
        /// </param>
        public ManagementPage(GUIManager gm)
        {

            /* Section for adding and updating an entry */

            Label updateLabel = new Label("Add/Update")
            {
                Font = this.Font.WithSize(16),
                TextAlignment = Alignment.Center,
                Wrap = WrapMode.Word
            };
            
            Label updateMessage =
                new Label("Here you can add or " +
                          "update a password entry. " +
                          "Simply enter the key you " +
                          "would like to associate a " +
                          "password to in the first box " +
                          "and enter the password in the " +
                          "second box. The password will " +
                          "then be hashed and stored.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Start,
                    Wrap = WrapMode.Word
                };

            // Add text
            this.Add(updateLabel, 0, 0, 2, 2, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(updateMessage, 0, 2, 2, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Text entries for Add/Update
            TextEntry updateKeyEntry = new TextEntry { PlaceholderText = "Enter key..." };
            PasswordEntry passwordEntry = new PasswordEntry { PlaceholderText = "Enter password..." };

            this.Add(updateKeyEntry, 0, 4, 1, 2, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(passwordEntry, 0, 5, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Update button
            Button updateButton = new Button("Add/Update");
            this.Add(updateButton, 0, 6, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            updateButton.Clicked += delegate
            {
                // Get text
                string key = updateKeyEntry.Text;
                string password = passwordEntry.Password;

                if (!GUIManager.IsValid(key) || !GUIManager.IsValid(password))
                {
                    // Not a valid user or password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entries!");
                    return;
                }

                // Check if database contains account
                if (DBManager.Instance.ContainsAccount(key))
                {
                    // If it does, update the password
                    DBManager.Instance.UpdateAccount(key, password);
                }
                else
                {
                    // Otherwise, add the account
                    DBManager.Instance.AddAccount(key, password);
                }
            };

            /* Section for getting or deleting an entry */

            Label getOrDeleteLabel = new Label("Get/Delete")
            {
                Font = this.Font.WithSize(16),
                TextAlignment = Alignment.Center,
                Wrap = WrapMode.Word
            };

            Label getOrDeleteMessage =
                new Label("Here you can get or delete " +
                          "an entry by entering in the " +
                          "key associated with the " +
                          "password you'd like to " +
                          "operate on. Then press 'Delete' " +
                          "to remove the entry from the " +
                          "database or press 'Get' to get " +
                          "the value from the database.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Start,
                    Wrap = WrapMode.Word
                };

            this.Add(getOrDeleteLabel, 0, 8, 2, 2, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(getOrDeleteMessage, 0, 10, 2, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Text entries for Get/Delete            
            TextEntry getOrDeleteKeyEntry = new TextEntry { PlaceholderText = "Enter key..." };
            TextEntry getOrDeleteResult = new TextEntry { PlaceholderText = "Waiting for command..." };
            getOrDeleteResult.Sensitive = false;

            this.Add(getOrDeleteKeyEntry, 0, 12, 1, 2, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(getOrDeleteResult, 0, 13, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Get button
            Button getButton = new Button("Get");
            this.Add(getButton, 0, 14, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Delete button
            Button deleteButton = new Button("Delete");
            this.Add(deleteButton, 0, 15, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            getButton.Clicked += delegate
            {

                string key = getOrDeleteKeyEntry.Text;

                if (!GUIManager.IsValid(key))
                {
                    // Not a valid user or password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entry!");
                    return;
                }

                // Find password associated with the key and show
                Dictionary<string, string> accountData = DBManager.Instance.GetAccount(key);
                string password;
                if (accountData.TryGetValue("Password", out password))
                {
                    getOrDeleteResult.Text = password;
                }
                else
                {
                    getOrDeleteResult.Text = "No account with the given key was found in the database.";
                }
            };
            deleteButton.Clicked += delegate
            {

                string key = getOrDeleteKeyEntry.Text;

                if (!GUIManager.IsValid(key))
                {
                    // Not a valid user or password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entry!");
                    return;
                }

                // Delete the account
                if (DBManager.Instance.RemoveAccount(key))
                {
                    getOrDeleteResult.Text = "Entry associated with " + key + " deleted!";
                }
                else
                {
                    getOrDeleteResult.Text = "No account with the given key was found in the database.";
                }
            };

            gm.SetLogoutButton();
        }
    }
}
