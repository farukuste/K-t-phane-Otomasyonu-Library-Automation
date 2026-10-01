using System;
using System.Collections.Generic;
using System.Linq;
using Final_Kütüphane_Projesi.Models;
using Final_Kütüphane_Projesi.Services;
using Final_Kütüphane_Projesi.Utilities;

namespace Final_Kütüphane_Projesi.Managers
{
    /// <summary>
    /// Kütüphane otomasyon sisteminin iş mantığını (Business Logic) yürüten ana sınıf.
    /// (OOP: SOLID Prensipleri, Encapsulation, Generic List Kullanımı, Try-Catch)
    /// </summary>
    public class LibraryManager
    {
        private readonly FileManager fileManager;
        private readonly LoggerService loggerService;

        // Bellekte tutulan veri listeleri (Generic List Kullanımı)
        public List<Person> Users { get; private set; }
        public List<Book> Books { get; private set; }
        public List<BorrowRecord> Borrows { get; private set; }
        public List<ReservationRecord> Reservations { get; private set; }
        public List<PenaltyRecord> Penalties { get; private set; }
        public List<Notification> Notifications { get; private set; }
        public List<FavoriteRecord> Favorites { get; private set; }
        public List<ReviewRecord> Reviews { get; private set; }

        // Kurallar
        public const int MaxBorrowLimit = 5; // Bir öğrencinin alabileceği maksimum kitap sayısı
        public const double DailyPenaltyRate = 5.0; // Günlük gecikme cezası (TL)

        public LibraryManager()
        {
            fileManager = new FileManager();
            loggerService = new LoggerService();

            // Verileri dosyadan yükle (Exception Handling)
            try
            {
                Users = fileManager.LoadUsers();
                Books = fileManager.LoadBooks();
                Borrows = fileManager.LoadBorrowRecords();
                Reservations = fileManager.LoadReservationRecords();
                Penalties = fileManager.LoadPenaltyRecords();
                Notifications = fileManager.LoadNotifications();
                Favorites = fileManager.LoadFavorites();
                Reviews = fileManager.LoadReviews();

                // Eğer kullanıcı yoksa (ilk çalıştırma), varsayılan verileri oluştur
                if (Users.Count == 0)
                {
                    // Şifreler otomatik olarak SHA256 ile şifrelenecektir (SecurityHelper)
                    RegisterAdmin("Yönetici Hesap", "admin", "admin123", "Süper Yönetici", "05555555555", "admin@kutuphane.com");
                    RegisterStudent("Ahmet Yılmaz", "ahmet", "ahmet123", "220101001", "05333333333", "ahmet@ogr.com");
                    RegisterStudent("Ayşe Demir", "ayse", "ayse123", "220101002", "05444444444", "ayse@ogr.com");

                    // Varsayılan Kitaplar
                    AddBook("BK-101", "Suç ve Ceza", "Fyodor Dostoyevski", "Dünya Klasikleri", 430, "A-12", 3);
                    AddBook("BK-102", "Nutuk", "Mustafa Kemal Atatürk", "Tarih & Siyaset", 540, "B-03", 5);
                    AddBook("BK-103", "Simyacı", "Paulo Coelho", "Felsefe & Roman", 184, "C-09", 0); // Stok 0: Rezervasyon testi için
                    AddBook("BK-104", "1984", "George Orwell", "Bilim Kurgu & Distopya", 350, "D-05", 2);

                    SaveChanges();
                }

                // Öğrencilerin ödünç aldığı aktif kitap listelerini ve cezalarını bağla
                SyncStudentData();

                // Geciken kitapları ve cezalarını otomatik güncelle
                UpdateOverduePenalties();
            }
            catch (Exception ex)
            {
                loggerService.WriteLog($"Sistem verileri yüklenirken hata oluştu: {ex.Message}");
                Users = new List<Person>();
                Books = new List<Book>();
                Borrows = new List<BorrowRecord>();
                Reservations = new List<ReservationRecord>();
                Penalties = new List<PenaltyRecord>();
                Notifications = new List<Notification>();
                Favorites = new List<FavoriteRecord>();
                Reviews = new List<ReviewRecord>();
            }
        }

        /// <summary>
        /// Bellekteki verileri .txt dosyalarına yazar.
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                fileManager.SaveUsers(Users);
                fileManager.SaveBooks(Books);
                fileManager.SaveBorrowRecords(Borrows);
                fileManager.SaveReservationRecords(Reservations);
                fileManager.SavePenaltyRecords(Penalties);
                fileManager.SaveNotifications(Notifications);
                fileManager.SaveFavorites(Favorites);
                fileManager.SaveReviews(Reviews);
            }
            catch (Exception ex)
            {
                loggerService.WriteLog($"Değişiklikler kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        #region Öğrenci Veri Senkronizasyonu
        private void SyncStudentData()
        {
            foreach (var user in Users)
            {
                if (user is Student student)
                {
                    // Aktif ödünçleri bağla
                    student.BorrowedBooks = Borrows
                        .Where(b => b.Username.Equals(student.Username, StringComparison.OrdinalIgnoreCase) && !b.IsReturned)
                        .Select(b => b.BookID)
                        .ToList();

                    // Ödenmemiş toplam cezayı bağla
                    student.PenaltyBalance = Penalties
                        .Where(p => p.Username.Equals(student.Username, StringComparison.OrdinalIgnoreCase) && !p.IsPaid)
                        .Sum(p => p.Amount);
                }
            }
        }

        /// <summary>
        /// Günlük ceza hesaplamasını otomatik yapar ve dusen.txt dosyasına yazar.
        /// </summary>
        public void UpdateOverduePenalties()
        {
            bool hasChanges = false;
            DateTime today = DateTime.Today;

            foreach (var borrow in Borrows.Where(b => !b.IsReturned))
            {
                if (today > borrow.DueDate.Date)
                {
                    int overdueDays = (today - borrow.DueDate.Date).Days;
                    double penaltyAmount = overdueDays * DailyPenaltyRate;

                    if (penaltyAmount > 0)
                    {
                        borrow.PenaltyAmount = penaltyAmount;
                        
                        // Ceza kaydını bul veya oluştur
                        var penaltyRecord = Penalties.FirstOrDefault(p => p.BookID == borrow.BookID && p.Username == borrow.Username && !p.IsPaid);
                        if (penaltyRecord != null)
                        {
                            if (Math.Abs(penaltyRecord.Amount - penaltyAmount) > 0.01)
                            {
                                penaltyRecord.Amount = penaltyAmount;
                                penaltyRecord.AccruedDate = today;
                                hasChanges = true;
                            }
                        }
                        else
                        {
                            string newId = Guid.NewGuid().ToString().Substring(0, 8);
                            Penalties.Add(new PenaltyRecord(newId, borrow.Username, borrow.BookID, penaltyAmount, today, false));
                            hasChanges = true;
                        }
                    }
                }
            }

            if (hasChanges)
            {
                SyncStudentData();
                SaveChanges();
            }
        }
        #endregion

        #region Kullanıcı Yönetimi (Giriş/Kayıt)
        public Person? Authenticate(string username, string password)
        {
            var user = Users.FirstOrDefault(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                loggerService.WriteLog($"Hatalı Giriş Denemesi: '{username}' adında bir kullanıcı bulunamadı.");
                return null;
            }

            if (user.Login(password)) // OOP: Polymorphism (Student veya Admin'e göre farklı Login tetiklenir)
            {
                SessionManager.SetSession(user);
                loggerService.WriteLog($"{user.Name} ({user.Username}) sisteme giriş yaptı.");
                return user;
            }
            
            loggerService.WriteLog($"Hatalı Giriş Denemesi: {user.Username} için yanlış şifre girildi.");
            return null;
        }

        public bool RegisterStudent(string name, string username, string password, string studentNumber, string phone, string email)
        {
            // Kullanıcı adı benzersiz olmalıdır
            if (Users.Any(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Bu kullanıcı adı sistemde zaten kayıtlı.");
            }

            // Şifre SHA256 ile hashlenerek saklanır
            string hashedPassword = SecurityHelper.ComputeSha256Hash(password);

            var student = new Student(name, username.Trim(), hashedPassword, studentNumber, phone, email);
            Users.Add(student);
            SaveChanges();

            loggerService.WriteLog($"Yeni Öğrenci Kaydı: {name} ({username}) sisteme kaydoldu.");
            SendNotification(username, "Kütüphane sistemimize hoş geldiniz! Keyifli okumalar dileriz.");
            return true;
        }

        public bool RegisterAdmin(string name, string username, string password, string permissionLevel, string phone, string email)
        {
            if (Users.Any(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Bu kullanıcı adı sistemde zaten kayıtlı.");
            }

            string hashedPassword = SecurityHelper.ComputeSha256Hash(password);
            var admin = new Admin(name, username.Trim(), hashedPassword, permissionLevel, phone, email);
            
            Users.Add(admin);
            SaveChanges();

            loggerService.WriteLog($"Yeni Yönetici Kaydı: {name} ({username}) oluşturuldu. Yetki: {permissionLevel}");
            return true;
        }
        #endregion

        #region Kitap Yönetimi
        public bool AddBook(string id, string name, string author, string category, int pageCount, string shelf, int stock)
        {
            if (Books.Any(b => b.BookID.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Bu Kitap ID zaten mevcut.");
            }

            var book = new Book(id.Trim().ToUpper(), name, author, category, pageCount, shelf, stock);
            Books.Add(book);
            SaveChanges();

            loggerService.WriteLog($"Kitap Eklendi: '{name}' (ID: {id}, Yazar: {author}, Stok: {stock})");

            // Eğer stok eklendiyse ve rezerve eden varsa, bildirim gönder
            if (stock > 0)
            {
                NotifyReservers(book);
            }

            return true;
        }

        public bool UpdateBook(string id, string name, string author, string category, int pageCount, string shelf, int stock)
        {
            var book = Books.FirstOrDefault(b => b.BookID.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
            if (book == null) return false;

            int oldStock = book.Stock;
            book.BookName = name;
            book.Author = author;
            book.Category = category;
            book.PageCount = pageCount;
            book.ShelfNumber = shelf;
            book.Stock = stock;

            SaveChanges();
            loggerService.WriteLog($"Kitap Güncellendi: '{name}' (ID: {id}, Yeni Stok: {stock})");

            // Stok artışı varsa rezerve edenleri bilgilendir
            if (stock > oldStock && stock > 0)
            {
                NotifyReservers(book);
            }

            return true;
        }

        public bool DeleteBook(string id)
        {
            var book = Books.FirstOrDefault(b => b.BookID.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
            if (book == null) return false;

            // Ödünçte kitap varsa silmeye izin verme
            if (Borrows.Any(b => b.BookID.Equals(id, StringComparison.OrdinalIgnoreCase) && !b.IsReturned))
            {
                throw new InvalidOperationException("Bu kitap şu anda ödünçte olduğu için silinemez.");
            }

            Books.Remove(book);
            
            // Rezervasyonları ve favorileri temizle (opsiyonel)
            Reservations.RemoveAll(r => r.BookID.Equals(id, StringComparison.OrdinalIgnoreCase));
            Favorites.RemoveAll(f => f.BookID.Equals(id, StringComparison.OrdinalIgnoreCase));

            SaveChanges();
            loggerService.WriteLog($"Kitap Silindi: '{book.BookName}' (ID: {id})");
            return true;
        }
        #endregion

        #region Gelişmiş Arama & Filtreleme (OOP: Method Overloading)
        
        /// <summary>
        /// Sadece kelimeyle arama yapar.
        /// (OOP: Metot Overloading)
        /// </summary>
        public List<Book> SearchBooks(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Books;
            query = query.ToLower();

            return Books.Where(b => b.BookName.ToLower().Contains(query) ||
                                    b.Author.ToLower().Contains(query) ||
                                    b.Category.ToLower().Contains(query) ||
                                    b.ShelfNumber.ToLower().Contains(query)).ToList();
        }

        /// <summary>
        /// Arama kelimesine ek olarak kategori filtresi uygular.
        /// (OOP: Metot Overloading)
        /// </summary>
        public List<Book> SearchBooks(string query, string category)
        {
            var results = SearchBooks(query);
            if (string.IsNullOrWhiteSpace(category) || category == "Tümü") return results;

            return results.Where(b => b.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Gelişmiş Filtreleme Seçenekleri.
        /// </summary>
        /// <param name="filterType">"Stock", "MostRead", "Newest", "All"</param>
        public List<Book> FilterBooks(List<Book> sourceList, string filterType)
        {
            switch (filterType)
            {
                case "Stock": // Stokta Olanlar
                    return sourceList.Where(b => b.IsAvailable).ToList();
                case "MostRead": // En çok okunanlar
                    var borrowCounts = Borrows.GroupBy(b => b.BookID)
                        .ToDictionary(g => g.Key, g => g.Count());
                    return sourceList.OrderByDescending(b => borrowCounts.ContainsKey(b.BookID) ? borrowCounts[b.BookID] : 0).ToList();
                case "Newest": // Yeni eklenenler (Kayıt sırası sondan başa)
                    var list = new List<Book>(sourceList);
                    list.Reverse();
                    return list;
                default:
                    return sourceList;
            }
        }
        #endregion

        #region Ödünç Alma & İade Etme Sistemi
        public bool BorrowBook(string username, string bookId)
        {
            var student = Users.OfType<Student>().FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            var book = Books.FirstOrDefault(b => b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase));

            if (student == null) throw new ArgumentException("Kullanıcı sistemde bulunamadı.");
            if (book == null) throw new ArgumentException("Kitap sistemde bulunamadı.");

            // Kural 1: Stok Kontrolü
            if (!book.IsAvailable)
            {
                throw new InvalidOperationException("Kitap şu anda stokta bulunmamaktadır. Dilerseniz rezerve edebilirsiniz.");
            }

            // Kural 2: Aynı kullanıcı aynı kitabı tekrar alamaz (aktif ödünç varsa)
            bool alreadyBorrowed = Borrows.Any(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                    b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && 
                                                    !b.IsReturned);
            if (alreadyBorrowed)
            {
                throw new InvalidOperationException("Bu kitap sizde zaten ödünç olarak bulunuyor.");
            }

            // Kural 3: Maksimum kitap limiti
            int activeBorrowCount = Borrows.Count(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !b.IsReturned);
            if (activeBorrowCount >= MaxBorrowLimit)
            {
                throw new InvalidOperationException($"Ödünç alma limitine ulaştınız. (Maksimum: {MaxBorrowLimit} adet)");
            }

            // Kural 4: Ceza borcu olanlar ödünç alamaz
            if (student.PenaltyBalance > 0)
            {
                throw new InvalidOperationException($"Ödenmemiş ceza borcunuz bulunmaktadır ({student.PenaltyBalance:C}). Borcunuzu ödemeden yeni kitap ödünç alamazsınız.");
            }

            // Ödünç İşlemi
            if (book.BorrowBook(username, 14)) // OOP: Method Overloading ve Arayüz kullanımı tetiklenir
            {
                string borrowId = Guid.NewGuid().ToString().Substring(0, 8);
                DateTime borrowDate = DateTime.Now;
                DateTime dueDate = borrowDate.AddDays(14); // 14 Günlük süre

                var record = new BorrowRecord(borrowId, username, bookId, borrowDate, dueDate, null);
                Borrows.Add(record);

                // Eğer aktif rezervasyon varsa ve bu kullanıcı rezervasyon sırasındaysa rezervasyonu kapat
                var res = Reservations.FirstOrDefault(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && r.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && r.IsActive);
                if (res != null)
                {
                    res.IsActive = false;
                }

                SyncStudentData();
                SaveChanges();

                loggerService.WriteLog($"{student.Name} ({student.Username}), '{book.BookName}' kitabını ödünç aldı. Son teslim tarihi: {dueDate:dd.MM.yyyy}");
                return true;
            }

            return false;
        }

        public bool ReturnBook(string username, string bookId)
        {
            var student = Users.OfType<Student>().FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            var book = Books.FirstOrDefault(b => b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase));
            var borrow = Borrows.FirstOrDefault(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                    b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && 
                                                    !b.IsReturned);

            if (student == null) throw new ArgumentException("Kullanıcı bulunamadı.");
            if (book == null) throw new ArgumentException("Kitap bulunamadı.");
            if (borrow == null) throw new InvalidOperationException("Bu kitaba ait aktif ödünç kaydı bulunamadı.");

            // İade İşlemi
            book.ReturnBook(); // Stok artar
            borrow.ReturnDate = DateTime.Now;

            // Gecikme kontrolü
            if (borrow.ReturnDate.Value.Date > borrow.DueDate.Date)
            {
                int overdueDays = (borrow.ReturnDate.Value.Date - borrow.DueDate.Date).Days;
                double penaltyAmount = overdueDays * DailyPenaltyRate;
                borrow.PenaltyAmount = penaltyAmount;

                // Ceza kaydını güncelle veya ekle
                var penaltyRecord = Penalties.FirstOrDefault(p => p.BookID == bookId && p.Username == username && !p.IsPaid);
                if (penaltyRecord != null)
                {
                    penaltyRecord.Amount = penaltyAmount;
                    penaltyRecord.AccruedDate = DateTime.Now;
                }
                else
                {
                    string newId = Guid.NewGuid().ToString().Substring(0, 8);
                    Penalties.Add(new PenaltyRecord(newId, username, bookId, penaltyAmount, DateTime.Now, false));
                }

                SendNotification(username, $"'{book.BookName}' kitabını {overdueDays} gün geciktirdiğiniz için {penaltyAmount:C} ceza yansıtılmıştır.");
            }

            SyncStudentData();
            SaveChanges();

            loggerService.WriteLog($"{student.Name} ({student.Username}), '{book.BookName}' kitabını iade etti.");

            // Kitap iade edildiğinde rezerve etmiş kişilere haber ver
            NotifyReservers(book);

            return true;
        }
        #endregion

        #region Rezervasyon Sistemi
        public bool ReserveBook(string username, string bookId)
        {
            var student = Users.OfType<Student>().FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            var book = Books.FirstOrDefault(b => b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase));

            if (student == null) throw new ArgumentException("Kullanıcı bulunamadı.");
            if (book == null) throw new ArgumentException("Kitap bulunamadı.");

            // Kural 1: Kitap stokta varsa rezerve edilemez
            if (book.Stock > 0)
            {
                throw new InvalidOperationException("Kitap stokta mevcuttur, doğrudan ödünç alabilirsiniz.");
            }

            // Kural 2: Zaten elinde olan kitabı rezerve edemez
            bool alreadyBorrowed = Borrows.Any(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                    b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && 
                                                    !b.IsReturned);
            if (alreadyBorrowed)
            {
                throw new InvalidOperationException("Zaten elinizde bulunan bir kitabı rezerve edemezsiniz.");
            }

            // Kural 3: Zaten aktif rezervasyonu var mı?
            bool alreadyReserved = Reservations.Any(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                        r.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && 
                                                        r.IsActive);
            if (alreadyReserved)
            {
                throw new InvalidOperationException("Bu kitap için zaten aktif bir rezervasyonunuz bulunmaktadır.");
            }

            // Rezervasyon oluştur
            string resId = Guid.NewGuid().ToString().Substring(0, 8);
            var reservation = new ReservationRecord(resId, username, bookId, DateTime.Now, true);
            Reservations.Add(reservation);

            SaveChanges();
            loggerService.WriteLog($"{student.Name} ({student.Username}), '{book.BookName}' kitabını rezerve etti.");
            return true;
        }

        public bool CancelReservation(string username, string bookId)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                              r.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase) && 
                                                              r.IsActive);
            if (reservation == null) return false;

            reservation.IsActive = false;
            SaveChanges();

            loggerService.WriteLog($"{username} kullanıcısı '{bookId}' kodlu kitap rezervasyonunu iptal etti.");
            return true;
        }

        private void NotifyReservers(Book book)
        {
            // Bu kitaba ait en eski tarihli aktif rezervasyonu bul
            var oldestActiveRes = Reservations
                .Where(r => r.BookID.Equals(book.BookID, StringComparison.OrdinalIgnoreCase) && r.IsActive)
                .OrderBy(r => r.ReservationDate)
                .FirstOrDefault();

            if (oldestActiveRes != null)
            {
                SendNotification(oldestActiveRes.Username, $"Rezerve ettiğiniz '{book.BookName}' kitabı stoklarımıza gelmiştir. Ödünç alabilirsiniz.");
            }
        }
        #endregion

        #region Bildirim Sistemi
        public void SendNotification(string username, string message)
        {
            string notId = Guid.NewGuid().ToString().Substring(0, 8);
            var notification = new Notification(notId, username, message, DateTime.Now, false);
            Notifications.Add(notification);
            SaveChanges();
        }

        public List<Notification> GetUserNotifications(string username)
        {
            return Notifications
                .Where(n => n.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(n => n.Date)
                .ToList();
        }

        public void MarkAllNotificationsAsRead(string username)
        {
            var userNots = Notifications.Where(n => n.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !n.IsRead);
            foreach (var not in userNots)
            {
                not.IsRead = true;
            }
            SaveChanges();
        }
        #endregion

        #region Ceza Ödeme / Geçmişi
        public bool PayPenalty(string username, string penaltyId)
        {
            var student = Users.OfType<Student>().FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            var penalty = Penalties.FirstOrDefault(p => p.PenaltyID.Equals(penaltyId, StringComparison.OrdinalIgnoreCase) && !p.IsPaid);

            if (student == null) throw new ArgumentException("Kullanıcı bulunamadı.");
            if (penalty == null) throw new InvalidOperationException("Aktif ceza kaydı bulunamadı.");

            penalty.IsPaid = true;

            // Ödünç tablosundaki ceza durumunu da güncelle
            var borrow = Borrows.FirstOrDefault(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                    b.BookID.Equals(penalty.BookID, StringComparison.OrdinalIgnoreCase) && 
                                                    b.PenaltyPaid == false);
            if (borrow != null)
            {
                borrow.PenaltyPaid = true;
            }

            SyncStudentData();
            SaveChanges();

            loggerService.WriteLog($"{student.Name} ({student.Username}), {penalty.Amount:C} tutarındaki kütüphane cezasını ödedi.");
            return true;
        }

        public bool PayAllPenalties(string username)
        {
            var student = Users.OfType<Student>().FirstOrDefault(s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (student == null) throw new ArgumentException("Kullanıcı bulunamadı.");

            var unpaidPenalties = Penalties.Where(p => p.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !p.IsPaid).ToList();
            if (unpaidPenalties.Count == 0) return false;

            double totalPaid = 0;
            foreach (var pen in unpaidPenalties)
            {
                pen.IsPaid = true;
                totalPaid += pen.Amount;

                var borrow = Borrows.FirstOrDefault(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                        b.BookID.Equals(pen.BookID, StringComparison.OrdinalIgnoreCase) && 
                                                        b.PenaltyPaid == false);
                if (borrow != null)
                {
                    borrow.PenaltyPaid = true;
                }
            }

            SyncStudentData();
            SaveChanges();

            loggerService.WriteLog($"{student.Name} ({student.Username}), toplam {totalPaid:C} ceza borcunu tamamen ödedi.");
            return true;
        }
        #endregion

        #region Favoriler Sistemi
        public bool AddToFavorites(string username, string bookId)
        {
            bool alreadyFav = Favorites.Any(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                 f.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase));
            if (alreadyFav) return false;

            Favorites.Add(new FavoriteRecord(username, bookId));
            SaveChanges();
            return true;
        }

        public bool RemoveFromFavorites(string username, string bookId)
        {
            var fav = Favorites.FirstOrDefault(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                                    f.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase));
            if (fav == null) return false;

            Favorites.Remove(fav);
            SaveChanges();
            return true;
        }

        public List<Book> GetUserFavorites(string username)
        {
            var favIds = Favorites
                .Where(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .Select(f => f.BookID)
                .ToList();

            return Books.Where(b => favIds.Contains(b.BookID)).ToList();
        }
        #endregion

        #region Yorumlar & Puanlama Sistemi
        public void AddReview(string username, string bookId, int rating, string comment)
        {
            var book = Books.FirstOrDefault(b => b.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase));
            if (book == null) throw new ArgumentException("Kitap bulunamadı.");

            // Yorum kaydı ekle
            string revId = Guid.NewGuid().ToString().Substring(0, 8);
            var review = new ReviewRecord(revId, username, bookId, rating, comment, DateTime.Now);
            Reviews.Add(review);

            // Kitabın ortalama puanını güncelle
            book.RatingSum += rating;
            book.RatingCount += 1;

            SaveChanges();
            loggerService.WriteLog($"{username} kullanıcısı '{book.BookName}' kitabına {rating} puan verdi.");
        }

        public List<ReviewRecord> GetBookReviews(string bookId)
        {
            return Reviews
                .Where(r => r.BookID.Equals(bookId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.Date)
                .ToList();
        }

        public bool DeleteReview(string reviewId)
        {
            var review = Reviews.FirstOrDefault(r => r.ReviewID.Equals(reviewId, StringComparison.OrdinalIgnoreCase));
            if (review == null) return false;

            // Kitap puanlarını güncelle
            var book = Books.FirstOrDefault(b => b.BookID.Equals(review.BookID, StringComparison.OrdinalIgnoreCase));
            if (book != null)
            {
                book.RatingSum -= review.Rating;
                book.RatingCount -= 1;
                if (book.RatingCount < 0) book.RatingCount = 0;
                if (book.RatingSum < 0) book.RatingSum = 0;
            }

            Reviews.Remove(review);
            SaveChanges();
            loggerService.WriteLog($"Yorum silindi (ID: {reviewId})");
            return true;
        }
        #endregion

        #region İstatistikler Sistemi
        public Dictionary<string, object> GetDashboardStatistics()
        {
            var stats = new Dictionary<string, object>();

            stats["TotalUsers"] = Users.Count;
            stats["TotalBooks"] = Books.Count;
            stats["TotalStock"] = Books.Sum(b => b.Stock);
            stats["ActiveBorrows"] = Borrows.Count(b => !b.IsReturned);
            stats["OverdueBooks"] = Borrows.Count(b => !b.IsReturned && DateTime.Today > b.DueDate.Date);

            // En çok okunan kitap
            var mostReadBookId = Borrows.GroupBy(b => b.BookID)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
            var mostReadBook = Books.FirstOrDefault(b => b.BookID == mostReadBookId);
            stats["MostReadBook"] = mostReadBook != null ? $"{mostReadBook.BookName} ({Borrows.Count(b => b.BookID == mostReadBookId)} kez)" : "Yok";

            // En aktif kullanıcı
            var mostActiveUser = Borrows.GroupBy(b => b.Username)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
            var activeUser = Users.FirstOrDefault(u => u.Username.Equals(mostActiveUser, StringComparison.OrdinalIgnoreCase));
            stats["MostActiveUser"] = activeUser != null ? $"{activeUser.Name} ({Borrows.Count(b => b.Username == mostActiveUser)} işlem)" : "Yok";

            // En çok rezervasyon yapılan kitap
            var mostReservedBookId = Reservations.GroupBy(r => r.BookID)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
            var mostReservedBook = Books.FirstOrDefault(b => b.BookID == mostReservedBookId);
            stats["MostReservedBook"] = mostReservedBook != null ? $"{mostReservedBook.BookName} ({Reservations.Count(r => r.BookID == mostReservedBookId)} kez)" : "Yok";

            return stats;
        }
        #endregion
    }
}
