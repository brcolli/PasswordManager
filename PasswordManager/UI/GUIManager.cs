using System;
using PasswordManager.Backend;
using Xwt;
using Application = Xwt.Application;
using MenuItem = Xwt.MenuItem;
using Window = Xwt.Window;
using Samples;

namespace PasswordManager.UI
{

    /// <summary>
    /// For designing and controlling the visual interface
    /// used to operate the Password Manager.
    /// </summary>

    class GUIManager
    {
        public GUIManager()
        {
            Application.Initialize(ToolkitType.Gtk);

            // Initialize main window
            MainWindow = new Window()
            {
                Title = "Password Manager",
                Width = 500,
                Height = 600,
                Resizable = true
            };

            LoggedIn = false;

            MainMenu = new Menu();

            // Creates the 'File' menu item
            MenuItem fileMenu = new MenuItem("File");
            fileMenu.SubMenu = new Menu();

            // Adds the 'Home' command to 'File' menu
            MenuItem homeCommand = new MenuItem(new Command("Home"));
            fileMenu.SubMenu.Items.Add(homeCommand);

            // Adds the 'Settings' command to 'File' menu
            MenuItem settingsCommand = new MenuItem(new Command("Settings"));
            fileMenu.SubMenu.Items.Add(settingsCommand);

            // Adds the 'Logout' command to 'File' menu
            MenuItem logoutCommand = new MenuItem(new Command("Logout"));
            fileMenu.SubMenu.Items.Add(logoutCommand);

            // Finalize fileMenu
            MainMenu.Items.Add((fileMenu));

            logoutCommand.Sensitive = false; // Change after logged in

            // Creates the 'Help' menu item
            MenuItem helpMenu = new MenuItem("Help");
            helpMenu.SubMenu = new Menu();

            // Adds 'About' item to 'Help' menu
            MenuItem aboutSubMenu = new MenuItem("About");
            helpMenu.SubMenu.Items.Add(aboutSubMenu);

            // Finalize helpMenu
            MainMenu.Items.Add(helpMenu);

            // Make login page
            LoginPage = new LoginPage(this);

            // Set buttons for window switching
            aboutSubMenu.Clicked += delegate
            {
                // Make about page
                AboutPage = new AboutPage(this);

                MainWindow.Content = AboutPage;
            };
            homeCommand.Clicked += delegate
            {
                // If not logged in, 'Home' sends to login page
                if (!LoggedIn)
                {
                    // Make login page
                    LoginPage = new LoginPage(this);
                    
                    MainWindow.Content = LoginPage;
                }
                else // Else send to home page
                {
                    // Make management page
                    ManagementPage = new ManagementPage(this);

                    MainWindow.Content = ManagementPage;
                }
            };
            logoutCommand.Clicked += delegate
            {
                LoggedIn = false;

                DBManager.Instance.CloseDB();

                // Make login page
                LoginPage = new LoginPage(this);

                MainWindow.Content = LoginPage;
            };
            settingsCommand.Clicked += delegate
            {
                SettingsPage settingsPage = new SettingsPage(this);

                MainWindow.Content = settingsPage;
            };

            MainWindow.Content = LoginPage; // First page

            MainWindow.MainMenu = MainMenu;
        }

        /// <summary>
        /// Toggles the logout button, depending on the current state of the user
        /// </summary>
        public void SetLogoutButton()
        {
            foreach (MenuItem i in MainMenu.Items)
            {
                if (i.Label == "File")
                {
                    foreach (MenuItem j in i.SubMenu.Items)
                    {
                        if (j.Label == "Logout")
                            j.Sensitive = LoggedIn; // Set to be the state of the user (logged in/logged out)
                    }
                }
            }
        }

        /// <summary>
        /// A string is considered valid if it isn't empty, does not contain
        /// the escape character, and does not contain any whitespace
        /// </summary>
        /// <param name="entry">
        /// The string to check validity
        /// </param>
        /// <returns>
        /// True if valid, else false
        /// </returns>
        public static Boolean IsValid(string entry)
        {
            if (entry.Contains("\\") || entry.Contains(" ") || entry == "")
                return false;
            return true;
        }

        /// <summary>
        /// Gets the SHA256 hash value from a string
        /// </summary>
        /// <param name="entry">
        /// String to hash
        /// </param>
        /// <returns>
        /// Hashed value of entry
        /// </returns>
//        public static string GetHash(string entry)
//        {
//            byte[] entryData = System.Text.Encoding.ASCII.GetBytes(entry);
//            entryData = new System.Security.Cryptography.SHA256Managed().ComputeHash(entryData);
//            return System.Text.Encoding.ASCII.GetString(entryData);
//        }

        /* Getters and setters */

        public Menu MainMenu { get; }
        public Window MainWindow { get; }

        // Pages
        public LoginPage LoginPage { get; set; }
        public AboutPage AboutPage { get; set; }
        public ManagementPage ManagementPage { get; set; }

        public Boolean LoggedIn { get; set; }
    }
}
