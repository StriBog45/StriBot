using StriBot.DryIoc;
using System;
using System.Windows.Forms;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GlobalContainer.Initialize();
            Application.Run(new Form1());
        }
    }
}