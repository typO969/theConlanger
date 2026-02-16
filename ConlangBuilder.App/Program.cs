using System;
using System.Windows.Forms;

namespace ConlangBuilder
{
    internal static class Program
    {
        // Plan:
        // - Resolve CS0121 (ambiguous call) by fully qualifying types.
        // - Explicitly call ConlangBuilder.ApplicationConfiguration.Initialize().
        // - Explicitly call System.Windows.Forms.Application.Run(new ConlangBuilder.MainForm()).

        [STAThread]
        static void Main()
        {
            ConlangBuilder.ApplicationConfiguration.initialize();
            System.Windows.Forms.Application.Run(new ConlangBuilder.MainForm());
        }
    }
}