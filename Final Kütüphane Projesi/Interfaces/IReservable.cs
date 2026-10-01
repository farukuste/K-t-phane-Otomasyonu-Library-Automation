namespace Final_Kütüphane_Projesi.Interfaces
{
    /// <summary>
    /// Rezervasyon yapılabilir nesneler için kontrat tanımı.
    /// (OOP: Interface Kullanımı)
    /// </summary>
    public interface IReservable
    {
        /// <summary>
        /// Nesne için rezervasyon kaydı oluşturur.
        /// </summary>
        /// <param name="username">Rezerve eden kullanıcının kullanıcı adı</param>
        /// <returns>Rezervasyon başarılı ise true, aksi halde false</returns>
        bool Reserve(string username);

        /// <summary>
        /// Nesne üzerindeki rezervasyonu iptal eder.
        /// </summary>
        /// <param name="username">Rezervasyonu iptal eden kullanıcının kullanıcı adı</param>
        /// <returns>İptal başarılı ise true, aksi halde false</returns>
        bool CancelReservation(string username);
    }
}
