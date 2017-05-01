using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// The page responsible for retrieving the passwords,
    /// editing the passwords, deleting, etc.
    /// </summary>

    class ManagementPage: Canvas
    {

        /// <summary>
        /// The management page
        /// </summary>
        /// <param name="gm">
        /// The manager
        /// </param>
        public ManagementPage(GUIManager gm)
        {

            // Section for adding and updating an entry
            Label updateLabel = new Label("Add/Update")
            {
                Font = this.Font.WithSize(20),
                TextAlignment = Alignment.Center
            };
            
            Label updateMessage =
                new Label("Here you can add or\n" +
                          "update a password entry.\n" +
                          "Simply enter the key you\n" +
                          "would like to associate a\n" +
                          "password to in the first box\n" +
                          "and enter the password in the\n" +
                          "second box. The password will\n" +
                          "then be hashed and stored.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Start
                };

            // Add text
            this.AddChild(updateLabel, new Rectangle(40, 0, 400, 50));
            this.AddChild(updateMessage, new Rectangle(300, 70, 180, 140));

            // Section for getting or deleting an entry
            Label getOrDeleteLabel = new Label("Get/Delete")
            {
                Font = this.Font.WithSize(20),
                TextAlignment = Alignment.Center
            };

            Label getOrDeleteMessage =
                new Label("Here you can get or delete\n" +
                          "an entry by entering in the\n" +
                          "key associated with the\n" +
                          "password you'd like to\n" +
                          "operate on. Then press 'Delete'\n" +
                          "to remove the entry from the\n" +
                          "database or press 'Get' to get\n" +
                          "the value from the database.")
                {
                    Font = this.Font.WithSize(10),
                    TextAlignment = Alignment.Start
                };

            this.AddChild(getOrDeleteLabel, new Rectangle(40, 225, 400, 50));
            this.AddChild(getOrDeleteMessage, new Rectangle(300, 295, 180 ,140));

            gm.SetLogoutButton();
        }
    }
}
