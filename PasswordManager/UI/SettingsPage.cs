using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// For now, simply controls the master password
    /// </summary>

    class SettingsPage: Canvas
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

            this.AddChild(settingsTitle, new Rectangle(40, 0, 400, 100));
            this.AddChild(settingsMessage, new Rectangle(90, 50, 300, 100));

            // Text for entries
            masterPasswordLabel = new Label("Master Password:");

            this.AddChild(masterPasswordLabel, new Rectangle(5, 225, 100, 50));

            if (gm.LoggedIn)
                masterPasswordEntry = new PasswordEntry {PlaceholderText = "Enter master password..."};
            else
            {
                masterPasswordEntry = new PasswordEntry { PlaceholderText = "Please login first..." };
                masterPasswordEntry.Sensitive = false;
            }
            
            this.AddChild(masterPasswordEntry, new Rectangle(105, 225, 280, 50));

            // Update master password button
            updatePasswordButton = new Button("Update Password");
            this.AddChild(updatePasswordButton, new Rectangle(105, 265, 50, 30));

            if (!gm.LoggedIn)
                updatePasswordButton.Sensitive = false;

            confirmUpdateButton = new Button("Update Password");

            updatePasswordButton.Clicked += delegate
            {

                // Get entered text
                string masterPassword = masterPasswordEntry.Password;

                if (!GUIManager.IsValid(masterPassword))
                {
                    // Not a valid password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entry!");
                    return;
                }

                // Remove old label, add new labels
                this.RemoveChild(masterPasswordLabel);
                oldPasswordLabel = new Label("Old Password:");
                newPasswordLabel = new Label("New Password:");

                this.AddChild(oldPasswordLabel, new Rectangle(5, 225, 100, 50));
                this.AddChild(newPasswordLabel, new Rectangle(5, 255, 100, 50));

                // Delete old entry, add new entries
                this.RemoveChild(masterPasswordEntry);
                oldPasswordEntry = new PasswordEntry { PlaceholderText = "Please enter your old password..."};
                newPasswordEntry = new PasswordEntry { PlaceholderText = "Please enter the new password..." };
                confirmNewPasswordEntry = new PasswordEntry { PlaceholderText = "Please re-enter the new password..." };

                // Populate the first new password entry with the entry above (maybe don't? idfk)
                newPasswordEntry.Password = masterPassword;

                this.AddChild(oldPasswordEntry, new Rectangle(105, 225, 280, 50));
                this.AddChild(newPasswordEntry, new Rectangle(105, 255, 280, 50));
                this.AddChild(confirmNewPasswordEntry, new Rectangle(105, 285, 280, 50));

                // Delete old button and add new button
                this.RemoveChild(updatePasswordButton);
                this.AddChild(confirmUpdateButton, new Rectangle(105, 325, 50, 30));

                cancelButton = new Button("Cancel");
                this.AddChild(cancelButton, new Rectangle(155, 325, 50, 30));

                
                cancelButton.Clicked += delegate
                {
                    /* Convert back to old layout */
                    ConvertToOldLayout();
                };
            };
            confirmUpdateButton.Clicked += delegate
            {

                // Get text
                string oldPassword = oldPasswordEntry.Password;
                string newPassword = newPasswordEntry.Password;
                string confirmNewPassword = confirmNewPasswordEntry.Password;

                if (!GUIManager.IsValid(oldPassword) || !GUIManager.IsValid(newPassword) ||
                    !GUIManager.IsValid(confirmNewPassword))
                {
                    // Not a valid password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entry!");
                    return;
                }

                if (newPassword != confirmNewPassword)
                {
                    // Passwords don't match
                    MessageDialog.ShowError("Passwords don't match!");
                    return;
                }

                if (oldPassword == newPassword)
                {
                    // Trying to update with same password
                    MessageDialog.ShowError("Please update with a different password from the current one!");
                    return;
                }

                // plz remove
                MessageDialog.ShowMessage("New password is: " + newPassword);

                // TODO Find old master password in database, compare to entered password, then update if passed

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
            this.RemoveChild(oldPasswordLabel);
            this.RemoveChild(newPasswordLabel);

            this.AddChild(masterPasswordLabel, new Rectangle(5, 225, 100, 50));

            // Delete entries and replace
            this.RemoveChild(oldPasswordEntry);
            this.RemoveChild(newPasswordEntry);
            this.RemoveChild(confirmNewPasswordEntry);

            masterPasswordEntry.Password = "";
            this.AddChild(masterPasswordEntry, new Rectangle(105, 225, 280, 50));

            // Delete button and replace
            this.RemoveChild(confirmUpdateButton);
            this.RemoveChild(cancelButton);

            this.AddChild(updatePasswordButton, new Rectangle(105, 265, 50, 30));
        }
    }
}

