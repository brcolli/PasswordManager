using System;
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
                Resizable = false
            };

            LoggedIn = false;

            MainMenu = new Menu();

            // Creates the 'File' menu item
            MenuItem fileMenu = new MenuItem("File");
            fileMenu.SubMenu = new Menu();

            // Adds the 'Home' command to 'File' menu
            MenuItem homeCommand = new MenuItem(new Command("Home"));
            fileMenu.SubMenu.Items.Add(homeCommand);

            // Adds the 'Settings' item to 'File' menu
            MenuItem settingsSubMenu = new MenuItem("Settings");
            fileMenu.SubMenu.Items.Add(settingsSubMenu);

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
                AboutPage aboutPage = new AboutPage(this);

                MainWindow.Content = aboutPage;
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
                    // Make about page
                    AboutPage aboutPage = new AboutPage(this); // TEMPORARY

                    MainWindow.Content = aboutPage;
                }
            };
            logoutCommand.Clicked += delegate
            {
                LoggedIn = false;

                // Make login page
                LoginPage = new LoginPage(this);

                MainWindow.Content = LoginPage;
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

        /* Getters and setters */

        public Menu MainMenu { get; }
        public Window MainWindow { get; }

        // Pages
        public LoginPage LoginPage { get; set; }
        public AboutPage AboutPage { get; set; }

        public Boolean LoggedIn { get; set; }
    }
}
