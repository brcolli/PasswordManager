/**
 * For designing and controlling the visual interface
 * used to operate the Password Manager.
 */

using System;
using Xwt;
using Application = Xwt.Application;
using MenuItem = Xwt.MenuItem;
using Window = Xwt.Window;
using Samples;

namespace PasswordManager.UI
{
    class GUIManager
    {
        private Menu mainMenu_;
        private Window mainWindow_;

        public GUIManager()
        {
            Application.Initialize(ToolkitType.Gtk);

            // Initialize main window
            mainWindow_ = new Window()
            {
                Title = "Password Manager",
                Width = 500,
                Height = 600,
                Resizable = false
            };

            mainMenu_ = new Menu();

            // Creates the 'File' menu item
            MenuItem fileMenu = new MenuItem("File");
            fileMenu.SubMenu = new Menu();

            // Adds the 'Settings' item to 'File' menu
            MenuItem settingsSubMenu = new MenuItem("Settings");
            fileMenu.SubMenu.Items.Add(settingsSubMenu);

            // Adds the 'Logout' command to 'File' menu
            MenuItem logoutCommand = new MenuItem(new Command("Logout"));
            fileMenu.SubMenu.Items.Add(logoutCommand);
            mainMenu_.Items.Add((fileMenu));

            logoutCommand.Sensitive = false; // Change after logged in

            // Creates the 'Help' menu item
            MenuItem helpMenu = new MenuItem("Help");
            helpMenu.SubMenu = new Menu();

            // Adds 'About' item to 'Help' menu
            MenuItem aboutSubMenu = new MenuItem("About");
            helpMenu.SubMenu.Items.Add(aboutSubMenu);
            mainMenu_.Items.Add(helpMenu);

            // Make login page
            LoginPage loginPage = new LoginPage();

            // Make about page
            AboutPage aboutPage = new AboutPage(mainWindow_);

            // Set buttons for window switching
            loginPage.LoginButton().Clicked += delegate
            {
                mainWindow_.Content = aboutPage;
            };

            mainWindow_.Content = loginPage; // First page

            mainWindow_.MainMenu = mainMenu_;
        }

        public Menu MainMenu() { return this.mainMenu_; }
        public Window MainWindow() { return this.mainWindow_; }
    }
}
