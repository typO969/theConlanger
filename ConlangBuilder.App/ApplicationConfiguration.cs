using System.Windows.Forms;

namespace ConlangBuilder
{
    internal static partial class ApplicationConfiguration
    {
        public static void initialize()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}