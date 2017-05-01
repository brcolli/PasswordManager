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
        /// <param name="gm">
        /// The manager
        /// </param>
        public AboutPage(GUIManager gm)
        {
            Label aboutLabel = new Label("About")
            {
                Font = this.Font.WithSize(20),
                TextAlignment = Alignment.Center
            };

            // About message
            Label aboutMessage =
                new Label("This Password Manager was designed for\n" +
                          "Spring 2017 CS 460's Final Project by\n" +
                          "Benjamin Collins (brcolli2) and Bradley Anderson (bsndrsn2).")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Center
                };

            // Add text
            this.AddChild(aboutLabel, new Rectangle(40, 0, 400, 100));
            this.AddChild(aboutMessage, new Rectangle(50, 50, 400, 100));

            gm.SetLogoutButton();
        }
    }
}
