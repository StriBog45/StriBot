using System;
using StriBot.DryIoc;

namespace StriBot
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            GlobalContainer.Initialize();
            System.Windows.Forms.Application.Run(new Form1());
        }
    }
}