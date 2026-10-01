namespace Final_Kütüphane_Projesi.Interfaces
{
    /// <summary>
    /// Loglama yapabilen sınıflar için kontrat tanımı.
    /// (OOP: Interface Kullanımı)
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Belirtilen mesajı log dosyasına kaydeder.
        /// </summary>
        /// <param name="message">Log mesajı</param>
        void WriteLog(string message);
    }
}
