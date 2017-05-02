using System;
using PasswordManager.Backend;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// For now, simply controls the master password
    /// </summary>

    class SettingsPage : Table
    {

        private Label masterPasswordLabel, oldPasswordLabel, newPasswordLabel;
        private PasswordEntry masterPasswordEntry, oldPasswordEntry, newPasswordEntry, confirmNewPasswordEntry;
        private Button updatePasswordButton, confirmUpdateButton, cancelButton;

        public SettingsPage(GUIManager gm)
        {

            Label settingsTitle = new Label("Settings")
            {
                Font = this.Font.WithSize(20),
                TextAlignment = Alignment.Center
            };

            // Welcome message
            Label settingsMessage =
                new Label("The master password determines how your " +
                          "passwords are encrypted. Here, you can " +
                          "choose to update your password.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Center,
                    Wrap = WrapMode.Word
                };

            this.Add(settingsTitle, 0, 0, 2, 2, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(settingsMessage, 0, 2, 2, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Text for entries
            masterPasswordLabel = new Label("Master Password:");

            this.Add(masterPasswordLabel, 0, 4, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            if (gm.LoggedIn)
                masterPasswordEntry = new PasswordEntry {PlaceholderText = "Enter master password..."};
            else
            {
                masterPasswordEntry = new PasswordEntry { PlaceholderText = "Please login first..." };
                masterPasswordEntry.Sensitive = false;
            }
            
            this.Add(masterPasswordEntry, 0, 5, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Update master password button
            updatePasswordButton = new Button("Update Password");
            this.Add(updatePasswordButton, 0, 6, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            if (!gm.LoggedIn)
                updatePasswordButton.Sensitive = false;

            confirmUpdateButton = new Button("Update Password");

            string masterPassword = "";

            updatePasswordButton.Clicked += delegate
            {

                // Get entered text
                masterPassword = masterPasswordEntry.Password;

                if (!GUIManager.IsValid(masterPassword))
                {
                    // Not a valid password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entry!");
                    return;
                }

                switch (DBManager.Instance.ValidatePassword(masterPassword))
                {
                    case 0:
                        MessageDialog.ShowError("Incorrect master password.");
                        return;
                    case 2:
                        MessageDialog.ShowError("No hash file found for database, cannot verify password.");
                        return;
                }

                // Remove old label, add new labels
                this.Remove(masterPasswordLabel);
                newPasswordLabel = new Label("New Password:");

                this.Add(newPasswordLabel, 0, 8, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

                // Delete old entry, add new entries
                this.Remove(masterPasswordEntry);
                newPasswordEntry = new PasswordEntry { PlaceholderText = "Please enter the new password..." };
                confirmNewPasswordEntry = new PasswordEntry { PlaceholderText = "Please re-enter the new password..." };

                newPasswordEntry.Password = "";

                this.Add(newPasswordEntry, 0, 10, 1, 2, ExpandHorizontal=true, ExpandVertical=false);
                this.Add(confirmNewPasswordEntry, 0, 11, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

                // Delete old button and add new button
                this.Remove(updatePasswordButton);
                this.Add(confirmUpdateButton, 0, 12, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

                cancelButton = new Button("Cancel");
                this.Add(cancelButton, 0, 13, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

                
                cancelButton.Clicked += delegate
                {
                    /* Convert back to old layout */
                    ConvertToOldLayout();
                };
            };
            confirmUpdateButton.Clicked += delegate
            {
                // Get text
                string newPassword = newPasswordEntry.Password;
                string confirmNewPassword = confirmNewPasswordEntry.Password;

                if (newPassword != confirmNewPassword)
                {
                    // Passwords don't match
                    MessageDialog.ShowError("Passwords don't match!");
                    return;
                }

                // Update master password
                switch (DBManager.Instance.ChangePassword(masterPassword, newPassword))
                {
                    case 0:
                        MessageDialog.ShowError("Incorrect master password.");
                        return;
                    case 2:
                        MessageDialog.ShowError("No database loaded.");
                        return;
                }

                /* Convert back to old layout */
                ConvertToOldLayout();
            };

            gm.SetLogoutButton();
        }

        /// <summary>
        /// Converts from new to old layout for Settings page
        /// </summary>
        private void ConvertToOldLayout()
        {

            // Delete labels and replace
            this.Remove(oldPasswordLabel);
            this.Remove(newPasswordLabel);

            this.Add(masterPasswordLabel, 0, 14, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Delete entries and replace
            this.Remove(oldPasswordEntry);
            this.Remove(newPasswordEntry);
            this.Remove(confirmNewPasswordEntry);

            masterPasswordEntry.Password = "";
            this.Add(masterPasswordEntry, 0, 15, 1, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Delete button and replace
            this.Remove(confirmUpdateButton);
            this.Remove(cancelButton);

            this.Add(updatePasswordButton, 0, 16, 1, 2, ExpandHorizontal=true, ExpandVertical=false);
        }
    }
}

