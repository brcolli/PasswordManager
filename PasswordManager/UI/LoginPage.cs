using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// The widget for the login page, with two simple
    /// text boxes; one textbox for the username,
    /// and the other for the password.
    /// </summary>

    class LoginPage: Canvas
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
                Font = this.Font.WithSize(20),
                TextAlignment = Alignment.Center
            };

            // Welcome message
            Label welcomeMessage =
                new Label("Your quick, simple local password storage device. \nPlease login or create a user below to begin.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Center
                };

            this.AddChild(welcomeHeader, new Rectangle(40, 0, 400, 100));
            this.AddChild(welcomeMessage, new Rectangle(90, 50, 300, 100));

            // Text for entries
            Label userLabel = new Label("User:");
            Label passwordLabel = new Label("Password:");

            this.AddChild(userLabel, new Rectangle(140, 200, 100, 100));
            this.AddChild(passwordLabel, new Rectangle(140, 230, 100, 100));

            // Text entries
            TextEntry userEntry = new TextEntry {PlaceholderText = "Enter username..."};
            PasswordEntry passwordEntry = new PasswordEntry {PlaceholderText = "Enter password..."};

            this.AddChild(userEntry, new Rectangle(200, 200, 100, 100));
            this.AddChild(passwordEntry, new Rectangle(200, 230, 100, 100));

            // Login button
            Button loginButton = new Button("Login");
            this.AddChild(loginButton, new Rectangle(200, 300, 50, 30));

            // Create user button
            Button createUserButton = new Button("Create User");
            this.AddChild(createUserButton, new Rectangle(250, 300, 80, 30));

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

                // Convert to SHA256
                string userHash = GUIManager.GetHash(user);
                string passwordHash = GUIManager.GetHash(password);

                // Temporary, plz delete
                MessageDialog.ShowMessage("User hash: " + userHash + "\n" + "Password hash: " + passwordHash);

                // TODO Find user in database, check password

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

                // Convert to SHA256
                string userHash = GUIManager.GetHash(user);
                string passwordHash = GUIManager.GetHash(password);

                // Temporary, plz delete
                MessageDialog.ShowMessage("User hash: " + userHash + "\n" + "Password hash: " + passwordHash);

                // TODO Add user/password to database

                // Make management page
                ManagementPage managementPage = new ManagementPage(gm);

                // In here, check for username/password validation and log in
                gm.MainWindow.Content = managementPage;
            };

            gm.SetLogoutButton();
        }
    }
}
