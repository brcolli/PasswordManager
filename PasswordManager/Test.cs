using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace PasswordManager
{
    class Test
    {
        [STAThread]
        static void Main()
        {
            Application.Initialize(ToolkitType.Gtk);
            // Application.Initialize("Xwt.GtkBackend.GtkEngine, Xwt.Gtk, Version=0.1.0.0");
            var mainWindow = new Window()
            {
                Title = "Xwt Test",
                Width = 500,
                Height = 400
            };
            mainWindow.Show();
            Application.Run();
            mainWindow.Dispose();
        }
    }
}
