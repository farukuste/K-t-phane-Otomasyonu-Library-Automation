using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Final_Kütüphane_Projesi.Models;

namespace Final_Kütüphane_Projesi.Services
{
    /// <summary>
    /// Flat file (.txt) tabanlı veri tabanı işlemlerini gerçekleştiren sınıf.
    /// (OOP: Encapsulation, Exception Handling - Hata Yönetimi, SOLID prensipleri)
    /// </summary>
    public class FileManager
    {
        private readonly string dataDirectory;

        // Dosya Yolları
        private readonly string usersPath;
        private readonly string booksPath;
        private readonly string borrowsPath;
        private readonly string reservationsPath;
        private readonly string penaltiesPath;
        private readonly string notificationsPath;
        private readonly string favoritesPath;
        private readonly string reviewsPath;

        public FileManager()
        {
            // Bu alanlar exception fırlatmayacağı için try bloğunun dışında güvenle atanabilir
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dataDirectory = Path.Combine(baseDirectory, "Data");

            usersPath = Path.Combine(dataDirectory, "kullanıcılar.txt");
            booksPath = Path.Combine(dataDirectory, "kitaplar.txt");
            borrowsPath = Path.Combine(dataDirectory, "odunckitap.txt");
            reservationsPath = Path.Combine(dataDirectory, "rezerve.txt");
            penaltiesPath = Path.Combine(dataDirectory, "dusen.txt");
            notificationsPath = Path.Combine(dataDirectory, "bildirimler.txt");
            favoritesPath = Path.Combine(dataDirectory, "favoriler.txt");
            reviewsPath = Path.Combine(dataDirectory, "puanlar_yorumlar.txt");

            try
            {
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                // Gerekli dosyaların boş olarak oluşturulması (Eğer yoklarsa)
                InitializeFile(usersPath);
                InitializeFile(booksPath);
                InitializeFile(borrowsPath);
                InitializeFile(reservationsPath);
                InitializeFile(penaltiesPath);
                InitializeFile(notificationsPath);
                InitializeFile(favoritesPath);
                InitializeFile(reviewsPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FileManager Başlatma Hatası: " + ex.Message);
            }
        }

        private void InitializeFile(string path)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
        }

        #region Kullanıcı Dosya İşlemleri (kullanıcılar.txt)
        public List<Person> LoadUsers()
        {
            var users = new List<Person>();
            try
            {
                if (!File.Exists(usersPath)) return users;

                string[] lines = File.ReadAllLines(usersPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 7) continue;

                    string role = parts[0];
                    string name = parts[1];
                    string username = parts[2];
                    string password = parts[3];
                    string extra = parts[4]; // StudentNo veya AdminPermission
                    string phone = parts[5];
                    string email = parts[6];
                    double balance = parts.Length > 7 ? double.Parse(parts[7], CultureInfo.InvariantCulture) : 0.0;

                    if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        users.Add(new Admin(name, username, password, extra, phone, email));
                    }
                    else if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
                    {
                        users.Add(new Student(name, username, password, extra, phone, email, balance));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Kullanıcıları Yükleme Hatası: " + ex.Message);
            }
            return users;
        }

        public void SaveUsers(List<Person> users)
        {
            try
            {
                var lines = new List<string>();
                foreach (var user in users)
                {
                    string role = user is Admin ? "Admin" : "Student";
                    string extra = user is Student student ? student.StudentNumber : ((Admin)user).PermissionLevel;
                    double balance = user is Student s ? s.PenaltyBalance : 0.0;

                    string line = $"{role}|{user.Name}|{user.Username}|{user.Password}|{extra}|{user.Phone}|{user.Email}|{balance.ToString(CultureInfo.InvariantCulture)}";
                    lines.Add(line);
                }
                File.WriteAllLines(usersPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Kullanıcıları Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Kitap Dosya İşlemleri (kitaplar.txt)
        public List<Book> LoadBooks()
        {
            var books = new List<Book>();
            try
            {
                if (!File.Exists(booksPath)) return books;

                string[] lines = File.ReadAllLines(booksPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 7) continue;

                    string id = parts[0];
                    string name = parts[1];
                    string author = parts[2];
                    string category = parts[3];
                    int pageCount = int.Parse(parts[4]);
                    string shelf = parts[5];
                    int stock = int.Parse(parts[6]);
                    
                    double ratingSum = parts.Length > 7 ? double.Parse(parts[7], CultureInfo.InvariantCulture) : 0.0;
                    int ratingCount = parts.Length > 8 ? int.Parse(parts[8]) : 0;

                    books.Add(new Book(id, name, author, category, pageCount, shelf, stock, ratingSum, ratingCount));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Kitapları Yükleme Hatası: " + ex.Message);
            }
            return books;
        }

        public void SaveBooks(List<Book> books)
        {
            try
            {
                var lines = new List<string>();
                foreach (var book in books)
                {
                    string line = $"{book.BookID}|{book.BookName}|{book.Author}|{book.Category}|{book.PageCount}|{book.ShelfNumber}|{book.Stock}|{book.RatingSum.ToString(CultureInfo.InvariantCulture)}|{book.RatingCount}";
                    lines.Add(line);
                }
                File.WriteAllLines(booksPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Kitapları Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Ödünç Kitap Dosya İşlemleri (odunckitap.txt)
        public List<BorrowRecord> LoadBorrowRecords()
        {
            var records = new List<BorrowRecord>();
            try
            {
                if (!File.Exists(borrowsPath)) return records;

                string[] lines = File.ReadAllLines(borrowsPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 5) continue;

                    string borrowId = parts[0];
                    string username = parts[1];
                    string bookId = parts[2];
                    DateTime borrowDate = DateTime.Parse(parts[3], CultureInfo.InvariantCulture);
                    DateTime dueDate = DateTime.Parse(parts[4], CultureInfo.InvariantCulture);
                    
                    DateTime? returnDate = null;
                    if (parts.Length > 5 && !string.IsNullOrWhiteSpace(parts[5]))
                    {
                        returnDate = DateTime.Parse(parts[5], CultureInfo.InvariantCulture);
                    }

                    double penalty = parts.Length > 6 ? double.Parse(parts[6], CultureInfo.InvariantCulture) : 0.0;
                    bool penaltyPaid = parts.Length > 7 && bool.Parse(parts[7]);

                    records.Add(new BorrowRecord(borrowId, username, bookId, borrowDate, dueDate, returnDate, penalty, penaltyPaid));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ödünç Kayıtlarını Yükleme Hatası: " + ex.Message);
            }
            return records;
        }

        public void SaveBorrowRecords(List<BorrowRecord> records)
        {
            try
            {
                var lines = new List<string>();
                foreach (var record in records)
                {
                    string retDateStr = record.ReturnDate.HasValue ? record.ReturnDate.Value.ToString(CultureInfo.InvariantCulture) : "";
                    string line = $"{record.BorrowID}|{record.Username}|{record.BookID}|{record.BorrowDate.ToString(CultureInfo.InvariantCulture)}|{record.DueDate.ToString(CultureInfo.InvariantCulture)}|{retDateStr}|{record.PenaltyAmount.ToString(CultureInfo.InvariantCulture)}|{record.PenaltyPaid}";
                    lines.Add(line);
                }
                File.WriteAllLines(borrowsPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ödünç Kayıtlarını Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Rezervasyon Dosya İşlemleri (rezerve.txt)
        public List<ReservationRecord> LoadReservationRecords()
        {
            var records = new List<ReservationRecord>();
            try
            {
                if (!File.Exists(reservationsPath)) return records;

                string[] lines = File.ReadAllLines(reservationsPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 5) continue;

                    string resId = parts[0];
                    string username = parts[1];
                    string bookId = parts[2];
                    DateTime resDate = DateTime.Parse(parts[3], CultureInfo.InvariantCulture);
                    bool isActive = bool.Parse(parts[4]);

                    records.Add(new ReservationRecord(resId, username, bookId, resDate, isActive));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Rezervasyonları Yükleme Hatası: " + ex.Message);
            }
            return records;
        }

        public void SaveReservationRecords(List<ReservationRecord> records)
        {
            try
            {
                var lines = new List<string>();
                foreach (var record in records)
                {
                    string line = $"{record.ReservationID}|{record.Username}|{record.BookID}|{record.ReservationDate.ToString(CultureInfo.InvariantCulture)}|{record.IsActive}";
                    lines.Add(line);
                }
                File.WriteAllLines(reservationsPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Rezervasyonları Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Düşen Ceza Dosya İşlemleri (dusen.txt)
        public List<PenaltyRecord> LoadPenaltyRecords()
        {
            var records = new List<PenaltyRecord>();
            try
            {
                if (!File.Exists(penaltiesPath)) return records;

                string[] lines = File.ReadAllLines(penaltiesPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 6) continue;

                    string penId = parts[0];
                    string username = parts[1];
                    string bookId = parts[2];
                    double amount = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    DateTime date = DateTime.Parse(parts[4], CultureInfo.InvariantCulture);
                    bool isPaid = bool.Parse(parts[5]);

                    records.Add(new PenaltyRecord(penId, username, bookId, amount, date, isPaid));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Cezaları Yükleme Hatası: " + ex.Message);
            }
            return records;
        }

        public void SavePenaltyRecords(List<PenaltyRecord> records)
        {
            try
            {
                var lines = new List<string>();
                foreach (var record in records)
                {
                    string line = $"{record.PenaltyID}|{record.Username}|{record.BookID}|{record.Amount.ToString(CultureInfo.InvariantCulture)}|{record.AccruedDate.ToString(CultureInfo.InvariantCulture)}|{record.IsPaid}";
                    lines.Add(line);
                }
                File.WriteAllLines(penaltiesPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Cezaları Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Bildirim Dosya İşlemleri (bildirimler.txt)
        public List<Notification> LoadNotifications()
        {
            var list = new List<Notification>();
            try
            {
                if (!File.Exists(notificationsPath)) return list;

                string[] lines = File.ReadAllLines(notificationsPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 5) continue;

                    string notId = parts[0];
                    string username = parts[1];
                    string msg = parts[2];
                    DateTime date = DateTime.Parse(parts[3], CultureInfo.InvariantCulture);
                    bool isRead = bool.Parse(parts[4]);

                    list.Add(new Notification(notId, username, msg, date, isRead));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Bildirimleri Yükleme Hatası: " + ex.Message);
            }
            return list;
        }

        public void SaveNotifications(List<Notification> list)
        {
            try
            {
                var lines = new List<string>();
                foreach (var not in list)
                {
                    string line = $"{not.NotificationID}|{not.Username}|{not.Message}|{not.Date.ToString(CultureInfo.InvariantCulture)}|{not.IsRead}";
                    lines.Add(line);
                }
                File.WriteAllLines(notificationsPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Bildirimleri Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Favori Kitap Dosya İşlemleri (favoriler.txt)
        public List<FavoriteRecord> LoadFavorites()
        {
            var list = new List<FavoriteRecord>();
            try
            {
                if (!File.Exists(favoritesPath)) return list;

                string[] lines = File.ReadAllLines(favoritesPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 2) continue;

                    list.Add(new FavoriteRecord(parts[0], parts[1]));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Favorileri Yükleme Hatası: " + ex.Message);
            }
            return list;
        }

        public void SaveFavorites(List<FavoriteRecord> list)
        {
            try
            {
                var lines = new List<string>();
                foreach (var fav in list)
                {
                    string line = $"{fav.Username}|{fav.BookID}";
                    lines.Add(line);
                }
                File.WriteAllLines(favoritesPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Favorileri Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion

        #region Puanlama ve Yorum Dosya İşlemleri (puanlar_yorumlar.txt)
        public List<ReviewRecord> LoadReviews()
        {
            var list = new List<ReviewRecord>();
            try
            {
                if (!File.Exists(reviewsPath)) return list;

                string[] lines = File.ReadAllLines(reviewsPath);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length < 6) continue;

                    string revId = parts[0];
                    string username = parts[1];
                    string bookId = parts[2];
                    int rating = int.Parse(parts[3]);
                    string comment = parts[4];
                    DateTime date = DateTime.Parse(parts[5], CultureInfo.InvariantCulture);

                    list.Add(new ReviewRecord(revId, username, bookId, rating, comment, date));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Yorumları Yükleme Hatası: " + ex.Message);
            }
            return list;
        }

        public void SaveReviews(List<ReviewRecord> list)
        {
            try
            {
                var lines = new List<string>();
                foreach (var rev in list)
                {
                    string line = $"{rev.ReviewID}|{rev.Username}|{rev.BookID}|{rev.Rating}|{rev.Comment}|{rev.Date.ToString(CultureInfo.InvariantCulture)}";
                    lines.Add(line);
                }
                File.WriteAllLines(reviewsPath, lines);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Yorumları Kaydetme Hatası: " + ex.Message);
            }
        }
        #endregion
    }
}
