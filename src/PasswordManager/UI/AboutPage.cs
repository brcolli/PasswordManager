using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// Page to display information about the program,
    /// such as developers, use, etc.
    /// </summary>

    class AboutPage: Table
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
                TextAlignment = Alignment.Center,
                Wrap = WrapMode.Word
            };

            // About message
            Label aboutMessage =
                new Label("This Password Manager was designed for " +
                          "Spring 2017 CS 460's Final Project by " +
                          "Benjamin Collins (brcolli2) and Bradley Anderson (bsndrsn2).")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Center,
                    Wrap = WrapMode.Word
                };

            // Add text
            this.Add(aboutLabel, 0, 0, 1, 1, ExpandHorizontal=true, ExpandVertical=false);
            this.Add(aboutMessage,0, 1, 1, 1, ExpandHorizontal=true, ExpandVertical=false);

            gm.SetLogoutButton();
        }
    }
}
