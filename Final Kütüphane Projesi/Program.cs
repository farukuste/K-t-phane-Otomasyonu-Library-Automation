using System;
using Final_Kütüphane_Projesi.Managers;
using Final_Kütüphane_Projesi.ConsoleUI;

namespace Final_Kütüphane_Projesi
{
    static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası (Konsol Arayüzü).
        /// </summary>
        static void Main(string[] args)
        {
            // İş mantığı sınıfını (Business Logic) başlat
            LibraryManager manager = new LibraryManager();

            // Konsol uygulamasını başlat
            ConsoleApp consoleApp = new ConsoleApp(manager);
            consoleApp.Start();
        }
    }
}
