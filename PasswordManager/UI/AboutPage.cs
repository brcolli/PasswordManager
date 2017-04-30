/**
 * Page to display information about the program,
 * such as developers, use, etc.
 */

using System;
using Xwt;

namespace PasswordManager.UI
{
    class AboutPage: Canvas
    {
        public AboutPage(Window mainWindow)
        {
            Label aboutLabel = new Label("About");
            this.AddChild(aboutLabel, new Rectangle(0, 0, 100, 100));

            // Set logout submenu to clickable
            foreach (MenuItem i in mainWindow.MainMenu.Items)
                Console.Write(i);
        }
    }
}
