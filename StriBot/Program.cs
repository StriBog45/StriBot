using System;
using Microsoft.Extensions.Configuration;
using StriBot.DryIoc;

namespace StriBot
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false);
            IConfiguration config = builder.Build();

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            GlobalContainer.Initialize(config);
            System.Windows.Forms.Application.Run(new Form1());
        }
    }
}