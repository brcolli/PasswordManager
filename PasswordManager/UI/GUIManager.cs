using System;
using System.Diagnostics;
using Xwt;
using Application = Xwt.Application;
using MenuItem = Xwt.MenuItem;
using Window = Xwt.Window;

/**
 * For designing and controlling the visual interface
 * used to operate the Password Manager.
 */

namespace PasswordManager.UI
{
    class GUIManager
    {

        [STAThread]
        static void Main()
        {
            Application.Initialize(ToolkitType.Gtk);

            // Initialize main window
            Window mainWindow = new Window()
            {
                Title = "Password Manager",
                Width = 500,
                Height = 600
            };

            Menu mainMenu = new Menu();

            // Adds a 'Help' tab to Main Menu
            MenuItem helpMenu = new MenuItem("Help");
            mainMenu.Items.Add(helpMenu);

            mainWindow.MainMenu = mainMenu;

            mainWindow.Show();
            Application.Run();
            mainWindow.Dispose();
        }
    }
}
