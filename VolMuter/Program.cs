//
// C#
// VolMuter.Program
// v 0.1, 25.09.2024
// https://github.com/dkxce/VolMuter
// en,ru,1251,utf-8
//

using System;
using System.Windows.Forms;

namespace VolMuter
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VolForm());
        }
    }
}
