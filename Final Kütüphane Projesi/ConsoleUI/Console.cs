using System;

namespace Final_Kütüphane_Projesi.ConsoleUI
{
    /// <summary>
    /// Headless/Sessiz ortamlarda Console.Clear(), Console.Title ve Console.ReadKey() 
    /// fonksiyonlarının çökmesini engelleyen güvenli sarmalayıcı (wrapper) sınıfı.
    /// </summary>
    internal static class Console
    {
        public static string Title
        {
            get
            {
                try 
                { 
                    if (OperatingSystem.IsWindows())
                        return System.Console.Title;
                    return "Kütüphane Otomasyonu";
                }
                catch { return "Kütüphane Otomasyonu"; }
            }
            set
            {
                try 
                { 
                    if (OperatingSystem.IsWindows())
                        System.Console.Title = value; 
                }
                catch { }
            }
        }

        public static void Clear()
        {
            try { System.Console.Clear(); }
            catch { }
        }

        public static string? ReadLine() => System.Console.ReadLine();

        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            try
            {
                return System.Console.ReadKey(intercept);
            }
            catch
            {
                // Headless test veya yönlendirilmiş girdi durumlarında Enter tuşu döner
                return new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false);
            }
        }

        public static void Write(string? value) => System.Console.Write(value);
        public static void WriteLine(string? value) => System.Console.WriteLine(value);
        public static void WriteLine() => System.Console.WriteLine();

        public static System.Text.Encoding OutputEncoding
        {
            get => System.Console.OutputEncoding;
            set
            {
                try { System.Console.OutputEncoding = value; }
                catch { }
            }
        }

        public static ConsoleColor ForegroundColor
        {
            get
            {
                try { return System.Console.ForegroundColor; }
                catch { return ConsoleColor.Gray; }
            }
            set
            {
                try { System.Console.ForegroundColor = value; }
                catch { }
            }
        }

        public static ConsoleColor BackgroundColor
        {
            get
            {
                try { return System.Console.BackgroundColor; }
                catch { return ConsoleColor.Black; }
            }
            set
            {
                try { System.Console.BackgroundColor = value; }
                catch { }
            }
        }

        public static void ResetColor()
        {
            try { System.Console.ResetColor(); }
            catch { }
        }

        public static bool IsInputRedirected
        {
            get
            {
                try { return System.Console.IsInputRedirected; }
                catch { return false; }
            }
        }
    }
}
