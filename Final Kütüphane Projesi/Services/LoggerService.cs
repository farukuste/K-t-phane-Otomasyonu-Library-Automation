using System;
using System.IO;
using Final_Kütüphane_Projesi.Interfaces;

namespace Final_Kütüphane_Projesi.Services
{
    /// <summary>
    /// Sistem loglarını dosyaya kaydeden servis sınıfı.
    /// (OOP: Interface Uygulaması, Exception Handling - Hata Yönetimi)
    /// </summary>
    public class LoggerService : ILoggable
    {
        private readonly string logFilePath;

        public LoggerService()
        {
            try
            {
                // Log dizinini ve dosyasını oluştur
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string logsDirectory = Path.Combine(baseDirectory, "Logs");

                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                logFilePath = Path.Combine(logsDirectory, "kayıtlar.txt");
            }
            catch (Exception ex)
            {
                // Hata durumunda debug konsoluna yazdır
                System.Diagnostics.Debug.WriteLine("LoggerService Başlatma Hatası: " + ex.Message);
                logFilePath = "kayıtlar.txt"; // Fallback
            }
        }

        /// <summary>
        /// Log kaydını dosyaya ekler.
        /// (OOP: Interface Metot Uygulaması, Exception Handling)
        /// </summary>
        public void WriteLog(string message)
        {
            try
            {
                string formattedMessage = $"[{DateTime.Now:dd.MM.yyyy HH:mm}] {message}{Environment.NewLine}";
                File.AppendAllText(logFilePath, formattedMessage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Log Yazma Hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Tüm log dosyasını okuyup döner (Arayüzde logları listelemek için).
        /// </summary>
        public string[] ReadAllLogs()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    return File.ReadAllLines(logFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Log Okuma Hatası: " + ex.Message);
            }
            return Array.Empty<string>();
        }
    }
}
