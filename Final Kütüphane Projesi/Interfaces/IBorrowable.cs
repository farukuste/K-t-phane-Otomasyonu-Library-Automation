namespace Final_Kütüphane_Projesi.Interfaces
{
    /// <summary>
    /// Ödünç alınabilir nesneler için kontrat tanımı.
    /// (OOP: Interface Kullanımı)
    /// </summary>
    public interface IBorrowable
    {
        /// <summary>
        /// Nesneyi belirli bir süreyle ödünç verir.
        /// </summary>
        /// <param name="username">Ödünç alan kullanıcının adı</param>
        /// <param name="durationDays">Ödünç süresi (gün)</param>
        /// <returns>İşlem başarılı ise true, aksi halde false</returns>
        bool Borrow(string username, int durationDays);

        /// <summary>
        /// Ödünç verilen nesneyi geri teslim alır.
        /// </summary>
        /// <returns>İşlem başarılı ise true, aksi halde false</returns>
        bool Return();
    }
}
