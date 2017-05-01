﻿using System;
using Xwt;

namespace PasswordManager.UI
{

    /// <summary>
    /// Runs the application.
    /// </summary>

    class PasswordManager
    {

        [STAThread]
        static void Main()
        {
            GUIManager manager = new GUIManager();

            // Run
            manager.MainWindow.Show();
            Application.Run();
            manager.MainWindow.Dispose();
        }
    }
}
