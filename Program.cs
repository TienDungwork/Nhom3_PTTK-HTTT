using System;
using System.Windows.Forms;
using LibraryManagement.Forms;
using LibraryManagement.Models;

namespace LibraryManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            while (true)
            {
                using (var login = new LoginForm())
                {
                    var loginResult = login.ShowDialog();
                    if (loginResult != DialogResult.OK || UserStore.CurrentUser == null)
                        break;
                }

                Form mainForm = UserStore.CurrentUser!.Role switch
                {
                    UserRole.ThuThu => new LibrarianForm(),
                    UserRole.DocGia => new ReaderForm(),
                    UserRole.Admin => new AdminForm(),
                    _ => new LibrarianForm()
                };

                using (mainForm)
                {
                    mainForm.ShowDialog();
                }

                // User clicked X on main form -> end app.
                // User clicked logout button -> CurrentUser is null -> continue loop to login.
                if (UserStore.CurrentUser != null)
                    break;
            }
        }
    }
}
