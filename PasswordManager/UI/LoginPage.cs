/**
 * 
 */

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
        private Button loginButton_;

        /// <summary>
        /// Login page
        /// </summary>
        /// <param name="mainMenu">
        /// The main menu item
        /// </param>
        /// <param name="loggedIn">
        /// State of user, for toggling 'Log' buttons
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
                new Label("Your quick, simple local password storage device. \nPlease login below to begin.")
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
            loginButton_ = new Button("Login");
            this.AddChild(loginButton_, new Rectangle(200, 300, 50, 30));

            loginButton_.Clicked += delegate
            {
                gm.LoggedIn = true;

                // Make about page
                AboutPage aboutPage = new AboutPage(gm); // TEMPORARY

                // In here, check for username/password validation and log in
                gm.MainWindow.Content = aboutPage;
            };

            gm.SetLogoutButton();
        }
    }
}
