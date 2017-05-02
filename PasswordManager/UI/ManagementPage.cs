using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// The page responsible for retrieving the passwords,
    /// editing the passwords, deleting, etc.
    /// </summary>

    class ManagementPage: Canvas
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
                Font = this.Font.WithSize(20),
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
                    TextAlignment = Alignment.Start
                };

            // Add text
            this.AddChild(updateLabel, new Rectangle(40, 0, 400, 50));
            this.AddChild(updateMessage, new Rectangle(300, 70, 180, 140));

            // Text entries for Add/Update
            TextEntry updateKeyEntry = new TextEntry { PlaceholderText = "Enter key..." };
            PasswordEntry passwordEntry = new PasswordEntry { PlaceholderText = "Enter password..." };

            this.AddChild(updateKeyEntry, new Rectangle(5, 60, 280, 100));
            this.AddChild(passwordEntry, new Rectangle(5, 90, 280, 100));

            // Update button
            Button updateButton = new Button("Add/Update");
            this.AddChild(updateButton, new Rectangle(5, 160, 80, 30));

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

                // Temporary, plz delete
                MessageDialog.ShowMessage("Key : " + key + "\n" + "Password : " + password);

                // TODO Look in database for key; if found, update, else add
            };

            /* Section for getting or deleting an entry */

            Label getOrDeleteLabel = new Label("Get/Delete")
            {
                Font = this.Font.WithSize(20),
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
                    TextAlignment = Alignment.Start
                };

            this.AddChild(getOrDeleteLabel, new Rectangle(40, 225, 400, 50));
            this.AddChild(getOrDeleteMessage, new Rectangle(300, 295, 180 ,140));

            // Text entries for Get/Delete            
            TextEntry getOrDeleteKeyEntry = new TextEntry { PlaceholderText = "Enter key..." };
            TextEntry getOrDeleteResult = new TextEntry { PlaceholderText = "Waiting for command..." };
            getOrDeleteResult.Sensitive = false;

            this.AddChild(getOrDeleteKeyEntry, new Rectangle(5, 285, 280, 100));
            this.AddChild(getOrDeleteResult, new Rectangle(5, 315, 280, 100));

            // Get button
            Button getButton = new Button("Get");
            this.AddChild(getButton, new Rectangle(5, 385, 50, 30));

            // Delete button
            Button deleteButton = new Button("Delete");
            this.AddChild(deleteButton, new Rectangle(55, 385, 50, 30));

            getButton.Clicked += delegate
            {

                string key = getOrDeleteKeyEntry.Text;

                if (!GUIManager.IsValid(key))
                {
                    // Not a valid user or password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entry!");
                    return;
                }

                // Temporary, plz delete
                MessageDialog.ShowMessage("Key : " + key);

                // TODO Find password associated with the key and show
                getOrDeleteResult.Text = "MyPassword";
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

                // Temporary, plz delete
                MessageDialog.ShowMessage("Key : " + key);

                // TODO Find data entry associated with the key and delete
                getOrDeleteResult.Text = "Entry associated with " + key + " deleted!";
            };

            gm.SetLogoutButton();
        }
    }
}
