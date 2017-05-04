using System;
using PasswordManager.Backend;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// The widget for the login page, with two simple
    /// text boxes; one textbox for the username,
    /// and the other for the password.
    /// </summary>

    class LoginPage: Table
    {

        /// <summary>
        /// Login page
        /// </summary>
        /// <param name="gm">
        /// The manager
        /// </param>
        public LoginPage(GUIManager gm)
        {
            // Welcome title
            Label welcomeHeader = new Label("Welcome to Password Manager!")
            {
                Font = this.Font.WithSize(16),
                TextAlignment = Alignment.Center,
                Wrap = WrapMode.Word,
            };

            // Welcome message
            Label welcomeMessage =
                new Label("Your quick, simple local password storage device. Please login or create a user below to begin.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Center, 
                    Wrap = WrapMode.Word
                };

            this.Add(welcomeHeader, 0, 0, 2, 2, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(welcomeMessage, 0, 2, 2, 2, ExpandHorizontal=true, ExpandVertical=false);

            // Text for entries
            Label userLabel = new Label("User:");
            Label passwordLabel = new Label("Password:");

            this.Add(userLabel, 0, 4, 1, 1, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(passwordLabel, 0, 5, 1, 1, ExpandHorizontal=true, ExpandVertical=false);

            // Text entries
            TextEntry userEntry = new TextEntry {PlaceholderText = "Enter username..."};
            PasswordEntry passwordEntry = new PasswordEntry {PlaceholderText = "Enter password..."};

            this.Add(userEntry, 1, 4, 1, 1, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(passwordEntry, 1, 5, 1, 1, ExpandHorizontal=true, ExpandVertical=false);

            // Login button
            Button loginButton = new Button("Login");
            this.Add(loginButton, 0, 6, 1, 1, ExpandHorizontal=true, ExpandVertical=false);

            // Create user button
            Button createUserButton = new Button("Create User");
            this.Add(createUserButton, 1, 6, 1, 1, ExpandHorizontal=true, ExpandVertical=false);

            loginButton.Clicked += delegate
            {
                gm.LoggedIn = true;

                // Get entered text
                string user = userEntry.Text;
                string password = passwordEntry.Password;

                if (!GUIManager.IsValid(user) || !GUIManager.IsValid(password))
                {
                    // Not a valid user or password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entries!");
                    return;
                }

                // Ensure user exists
                if (!DBManager.Instance.DBExists(user))
                {
                    MessageDialog.ShowError("User with the given name does not exist.");
                    return;
                }

                // Log in
                switch (DBManager.Instance.OpenDB(user, password))
                {
                    case 0:
                        MessageDialog.ShowError("Incorrect password provided.");
                        return;
                    case 2:
                        MessageDialog.ShowError("No password hash file was found! Database cannot be decrypted.");
                        return;
                    case 3:
                        MessageDialog.ShowError("Database open failed.");
                        return;
                    case 4:
                        MessageDialog.ShowError("No database found and failed to create one. This should be impossible.");
                        return;
                }

                // Make management page
                ManagementPage managementPage = new ManagementPage(gm);

                // In here, check for username/password validation and log in
                gm.MainWindow.Content = managementPage;
            };
            createUserButton.Clicked += delegate
            {
                gm.LoggedIn = true;

                // Get entered text
                string user = userEntry.Text;
                string password = passwordEntry.Password;

                if (!GUIManager.IsValid(user) || !GUIManager.IsValid(password))
                {
                    // Not a valid user or password
                    MessageDialog.ShowError("Please do not enter empty values, whitespace, or '\\' in the entries!");
                    return;
                }

                // Don't allow overwriting users
                if (DBManager.Instance.DBExists(user))
                {
                    MessageDialog.ShowError("User with the given name already exists.");
                    return;
                }

                // Log in
                switch (DBManager.Instance.OpenDB(user, password))
                {
                    case 0:
                        MessageDialog.ShowError("Incorrect password provided. This makes no sense.");
                        return;
                    case 2:
                        MessageDialog.ShowError("No password hash file was found! Database cannot be decrypted. This makes no sense.");
                        return;
                    case 3:
                        // This shouldn't happen
                        MessageDialog.ShowError("Our encryption scheme is broken!");
                        return;
                    case 4:
                        MessageDialog.ShowError("No database found and failed to create one. This should be impossible.");
                        return;
                }

                // Make management page
                ManagementPage managementPage = new ManagementPage(gm);

                // In here, check for username/password validation and log in
                gm.MainWindow.Content = managementPage;
            };

            gm.SetLogoutButton();
        }
    }
}
