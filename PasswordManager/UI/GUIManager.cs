using System;
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

            // Creates the 'Help' menu item
            MenuItem helpMenu = new MenuItem("Help");
            helpMenu.SubMenu = new Menu();
            
            // Adds 'About' command to 'Help' menu
            MenuItem aboutPage = new MenuItem(new Command("About"));
            helpMenu.SubMenu.Items.Add(aboutPage);
            mainMenu.Items.Add(helpMenu);

            aboutPage.Clicked += (sender, e) => MessageDialog.ShowMessage("'About' clicked.");
            
            mainWindow.MainMenu = mainMenu;

            mainWindow.Show();
            Application.Run();
            mainWindow.Dispose();
        }
    }
}
