using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// Page to display information about the program,
    /// such as developers, use, etc.
    /// </summary>

    class AboutPage: Canvas
    {

        /// <summary>
        /// Sets up the 'About' page
        /// </summary>
        /// <param name="mainMenu">
        /// The main menu item
        /// </param>
        /// <param name="loggedIn">
        /// State of user, for toggling 'Log' buttons
        /// </param>
        public AboutPage(GUIManager gm)
        {
            Label aboutLabel = new Label("About");
            this.AddChild(aboutLabel, new Rectangle(0, 0, 100, 100));

            gm.SetLogoutButton();
        }
    }
}
