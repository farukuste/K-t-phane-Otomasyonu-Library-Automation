using System;
using Final_Kütüphane_Projesi.Interfaces;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kütüphanedeki kitapları temsil eden sınıf.
    /// (OOP: Encapsulation - Kapsülleme, Interface Uygulaması)
    /// </summary>
    public class Book : IBorrowable, IReservable
    {
        private string bookId = string.Empty;
        private string bookName = string.Empty;
        private string author = string.Empty;
        private string category = string.Empty;
        private int pageCount;
        private string shelfNumber = string.Empty;
        private int stock;

        public string BookID
        {
            get => bookId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Kitap ID boş bırakılamaz.");
                bookId = value.Trim().ToUpper();
            }
        }

        public string BookName
        {
            get => bookName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Kitap adı boş bırakılamaz.");
                bookName = value.Trim();
            }
        }

        public string Author
        {
            get => author;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Yazar adı boş bırakılamaz.");
                author = value.Trim();
            }
        }

        public string Category
        {
            get => category;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Kategori boş bırakılamaz.");
                category = value.Trim();
            }
        }

        public int PageCount
        {
            get => pageCount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Sayfa sayısı 0'dan büyük olmalıdır.");
                pageCount = value;
            }
        }

        public string ShelfNumber
        {
            get => shelfNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Raf numarası boş bırakılamaz.");
                shelfNumber = value.Trim().ToUpper();
            }
        }

        public int Stock
        {
            get => stock;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Stok miktarı negatif olamaz.");
                stock = value;
            }
        }

        /// <summary>
        /// Kitabın ödünç alınabilir olup olmadığını belirtir.
        /// </summary>
        public bool IsAvailable => Stock > 0;

        // Puanlama Sistemi için eklenen özellikler
        public double RatingSum { get; set; } = 0;
        public int RatingCount { get; set; } = 0;
        public double AverageRating => RatingCount > 0 ? Math.Round(RatingSum / RatingCount, 1) : 0.0;

        /// <summary>
        /// Kitap Yapıcısı (OOP: Constructor Kullanımı)
        /// </summary>
        public Book(string bookId, string bookName, string author, string category, int pageCount, string shelfNumber, int stock, double ratingSum = 0.0, int ratingCount = 0)
        {
            BookID = bookId;
            BookName = bookName;
            Author = author;
            Category = category;
            PageCount = pageCount;
            ShelfNumber = shelfNumber;
            Stock = stock;
            RatingSum = ratingSum;
            RatingCount = ratingCount;
        }

        #region IBorrowable Arayüz Metotları
        public bool Borrow(string username, int durationDays)
        {
            if (!IsAvailable) return false;
            Stock--;
            return true;
        }

        public bool Return()
        {
            Stock++;
            return true;
        }
        #endregion

        #region IReservable Arayüz Metotları
        public bool Reserve(string username)
        {
            // Rezervasyon mantığı: Kitap zaten yoksa rezerve edilebilir.
            // Bu mantık LibraryManager tarafından koordine edilecektir.
            return true;
        }

        public bool CancelReservation(string username)
        {
            return true;
        }
        #endregion

        #region Metot Aşırı Yükleme (Method Overloading - OOP)
        
        /// <summary>
        /// Kitap ödünç verme işleminin parametresiz aşırı yüklemesi.
        /// (14 günlük standart süre ve sistem kullanıcısı ile ödünç alır)
        /// </summary>
        public bool BorrowBook()
        {
            return BorrowBook("SistemKullanicisi", 14);
        }

        /// <summary>
        /// Kitap ödünç verme işleminin parametreli aşırı yüklemesi.
        /// </summary>
        public bool BorrowBook(string username, int durationDays)
        {
            return Borrow(username, durationDays);
        }

        /// <summary>
        /// Kitap teslim alma işleminin parametresiz aşırı yüklemesi.
        /// </summary>
        public bool ReturnBook()
        {
            return Return();
        }

        /// <summary>
        /// Kitap teslim alma işleminin parametreli aşırı yüklemesi.
        /// </summary>
        public bool ReturnBook(string username)
        {
            return Return();
        }
        #endregion
    }
}
