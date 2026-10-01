using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Final_Kütüphane_Projesi.Managers;
using Final_Kütüphane_Projesi.Models;
using Final_Kütüphane_Projesi.Services;
using Final_Kütüphane_Projesi.Utilities;

namespace Final_Kütüphane_Projesi.ConsoleUI
{
    /// <summary>
    /// Kütüphane Otomasyonu Konsol Arayüzü Sınıfı.
    /// (OOP: Encapsulation, Exception Handling, Polymorphism, Abstraction)
    /// </summary>
    public class ConsoleApp
    {
        private readonly LibraryManager libraryManager;
        private readonly LoggerService loggerService;

        public ConsoleApp(LibraryManager manager)
        {
            this.libraryManager = manager;
            this.loggerService = new LoggerService();
        }

        public void Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Bilgi Kütüphanesi - Konsol Yönetim Paneli";

            while (true)
            {
                Console.Clear();
                PrintAppBanner();
                PrintHeader("ANA GİRİŞ MENÜSÜ");
                Console.WriteLine(" [1] Giriş Yap");
                Console.WriteLine(" [2] Öğrenci Kayıt Ol");
                Console.WriteLine(" [3] Uygulamadan Çık");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" Seçiminiz (1-3): ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (input == "1")
                {
                    LoginFlow();
                }
                else if (input == "2")
                {
                    RegisterFlow();
                }
                else if (input == "3")
                {
                    PrintInfo("Uygulamadan çıkılıyor. Değişiklikler kaydediliyor...");
                    libraryManager.SaveChanges();
                    break;
                }
                else
                {
                    PrintError("Geçersiz seçim! Lütfen tekrar deneyin.");
                    WaitUser();
                }
            }
        }

        #region Giriş & Kayıt Akışları
        private void LoginFlow()
        {
            Console.Clear();
            PrintAppBanner();
            PrintHeader("GİRİŞ YAP");

            Console.Write(" Kullanıcı Adı: ");
            string username = Console.ReadLine() ?? "";

            Console.Write(" Şifre: ");
            string password = ReadPassword();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                PrintError("Kullanıcı adı veya şifre boş geçilemez!");
                WaitUser();
                return;
            }

            try
            {
                // LibraryManager ile giriş doğrulaması yap
                var user = libraryManager.Authenticate(username, password);
                if (user != null)
                {
                    PrintSuccess($"Giriş Başarılı! Hoş geldiniz, {user.Name}.");
                    WaitUser();

                    if (SessionManager.IsAdmin)
                    {
                        AdminDashboardLoop();
                    }
                    else
                    {
                        StudentDashboardLoop();
                    }
                }
                else
                {
                    PrintError("Hatalı kullanıcı adı veya şifre!");
                    WaitUser();
                }
            }
            catch (Exception ex)
            {
                PrintError("Giriş esnasında hata oluştu: " + ex.Message);
                WaitUser();
            }
        }

        private void RegisterFlow()
        {
            Console.Clear();
            PrintAppBanner();
            PrintHeader("YENİ ÖĞRENCİ KAYDI");

            Console.Write(" Adınız Soyadınız: ");
            string name = Console.ReadLine() ?? "";

            Console.Write(" Kullanıcı Adınız: ");
            string username = Console.ReadLine() ?? "";

            Console.Write(" Şifreniz: ");
            string password = ReadPassword();

            Console.Write(" Öğrenci Numaranız: ");
            string studentNo = Console.ReadLine() ?? "";

            Console.Write(" Telefon Numaranız: ");
            string phone = Console.ReadLine() ?? "";

            Console.Write(" E-Posta Adresiniz: ");
            string email = Console.ReadLine() ?? "";

            try
            {
                libraryManager.RegisterStudent(name, username, password, studentNo, phone, email);
                loggerService.WriteLog($"Yeni öğrenci kaydı yapıldı: {name} ({username})");
                PrintSuccess("Öğrenci kaydı başarıyla oluşturulmuştur. Artık giriş yapabilirsiniz!");
            }
            catch (Exception ex)
            {
                PrintError("Kayıt oluşturulurken hata: " + ex.Message);
            }
            WaitUser();
        }

        private string ReadPassword()
        {
            if (Console.IsInputRedirected)
            {
                return Console.ReadLine() ?? "";
            }

            string password = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (key.KeyChar != '\u0000') // kontrol karakterleri hariç
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            return password;
        }
        #endregion

        #region Öğrenci Dashboard
        private void StudentDashboardLoop()
        {
            var student = (Student)SessionManager.CurrentUser!;

            // Öğrenci giriş yaptığında okunmamış bildirimleri göster
            ShowUnreadNotifications(student.Username);

            while (true)
            {
                Console.Clear();
                PrintAppBanner();
                
                // Öğrenci Profil Bilgisi
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" [ÖĞRENCİ PANELİ] Üye: {student.Name} | Öğrenci No: {student.StudentNumber}");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($" Gecikme Ceza Borcu: {student.PenaltyBalance:C} | Aktif Kitap Sayısı: {student.BorrowedBooks.Count}");
                Console.ResetColor();
                Console.WriteLine(new string('-', 70));

                PrintHeader("İŞLEMLER MENÜSÜ");
                Console.WriteLine(" [1] Kitap Ara & Katalog Listele");
                Console.WriteLine(" [2] Kitap Ödünç Al");
                Console.WriteLine(" [3] Kitap İade Et (Teslim Et)");
                Console.WriteLine(" [4] Kitap Rezerve Et");
                Console.WriteLine(" [5] Kitap Puanla & Yorum Yap");
                Console.WriteLine(" [6] Yorumları ve Değerlendirmeleri Gör");
                Console.WriteLine(" [7] Kitap Favorilere Ekle / Kaldır");
                Console.WriteLine(" [8] Favori Kitaplarımı Listele");
                Console.WriteLine(" [9] Bildirim Kutusunu Aç");
                Console.WriteLine(" [10] Gecikme Cezası Öde");
                Console.WriteLine(" [11] Oturumu Kapat");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" Seçiminiz (1-11): ");
                Console.ResetColor();

                string opt = Console.ReadLine() ?? "";
                if (opt == "1") StudentSearchBooks();
                else if (opt == "2") StudentBorrow();
                else if (opt == "3") StudentReturn();
                else if (opt == "4") StudentReserve();
                else if (opt == "5") StudentAddReview();
                else if (opt == "6") StudentViewReviews();
                else if (opt == "7") StudentManageFavorite();
                else if (opt == "8") StudentListFavorites();
                else if (opt == "9") StudentNotifications();
                else if (opt == "10") StudentPayPenalty();
                else if (opt == "11")
                {
                    loggerService.WriteLog($"{student.Name} oturumu kapattı.");
                    SessionManager.ClearSession();
                    PrintInfo("Oturum kapatıldı.");
                    WaitUser();
                    break;
                }
                else
                {
                    PrintError("Geçersiz seçim!");
                    WaitUser();
                }
            }
        }

        private void ShowUnreadNotifications(string username)
        {
            var nots = libraryManager.GetUserNotifications(username).Where(n => !n.IsRead).ToList();
            if (nots.Count > 0)
            {
                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($" ⚠️ OKUNMAMIŞ {nots.Count} YENİ BİLDİRİMİNİZ VAR! ");
                Console.ResetColor();
                foreach (var n in nots)
                {
                    Console.WriteLine($" -> [{n.Date:dd.MM.yyyy}] {n.Message}");
                }
                libraryManager.MarkAllNotificationsAsRead(username);
                Console.WriteLine();
                WaitUser();
            }
        }

        private void StudentSearchBooks()
        {
            Console.Clear();
            PrintHeader("KİTAP ARAMA & KATALOG LİSTELEME");
            Console.Write(" Arama terimi girin (Tümü için boş bırakın): ");
            string query = Console.ReadLine() ?? "";

            Console.Write(" Kategori girin (Tümü için boş bırakın): ");
            string cat = Console.ReadLine() ?? "";

            var books = libraryManager.SearchBooks(query, cat);

            Console.WriteLine("\n Filtre Seçenekleri:");
            Console.WriteLine(" [1] Filtreleme Yapma (Tümü)");
            Console.WriteLine(" [2] Yalnızca Stoktakiler");
            Console.WriteLine(" [3] En Çok Okunanlar");
            Console.WriteLine(" [4] Yeni Eklenenler");
            Console.Write(" Seçiminiz: ");
            string filterOpt = Console.ReadLine() ?? "1";

            if (filterOpt == "2") books = libraryManager.FilterBooks(books, "Stock");
            else if (filterOpt == "3") books = libraryManager.FilterBooks(books, "MostRead");
            else if (filterOpt == "4") books = libraryManager.FilterBooks(books, "Newest");

            Console.Clear();
            PrintHeader("ARAMA SONUÇLARI");
            DrawBookTable(books);
            WaitUser();
        }

        private void StudentBorrow()
        {
            Console.Clear();
            PrintHeader("KİTAP ÖDÜNÇ AL");
            Console.Write(" Ödünç almak istediğiniz Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            try
            {
                libraryManager.BorrowBook(SessionManager.CurrentUser!.Username, id);
                PrintSuccess("Kitap başarıyla ödünç alınmıştır! Teslim süreniz 14 gündür.");
            }
            catch (Exception ex)
            {
                PrintError("İşlem Başarısız: " + ex.Message);
            }
            WaitUser();
        }

        private void StudentReturn()
        {
            Console.Clear();
            PrintHeader("KİTAP İADE ET");
            
            // Aktif ödünç alınanları listele
            string username = SessionManager.CurrentUser!.Username;
            var activeBorrows = libraryManager.Borrows.Where(b => b.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !b.ReturnDate.HasValue).ToList();

            if (activeBorrows.Count == 0)
            {
                PrintInfo("Elinizde aktif ödünç kitap bulunmamaktadır.");
                WaitUser();
                return;
            }

            Console.WriteLine(" Elinizdeki Kitaplar:");
            foreach (var b in activeBorrows)
            {
                var book = libraryManager.Books.FirstOrDefault(x => x.BookID == b.BookID);
                Console.WriteLine($" -> [ID: {b.BookID}] {book?.BookName} (Yazar: {book?.Author}) - Son Teslim: {b.DueDate:dd.MM.yyyy}");
            }

            Console.WriteLine();
            Console.Write(" İade etmek istediğiniz Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            try
            {
                libraryManager.ReturnBook(username, id);
                PrintSuccess("Kitap başarıyla teslim alındı.");
            }
            catch (Exception ex)
            {
                PrintError("İade esnasında hata: " + ex.Message);
            }
            WaitUser();
        }

        private void StudentReserve()
        {
            Console.Clear();
            PrintHeader("KİTAP REZERVE ET");
            Console.Write(" Rezervasyon yapmak istediğiniz Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            try
            {
                libraryManager.ReserveBook(SessionManager.CurrentUser!.Username, id);
                PrintSuccess("Kitap rezervasyon sırasına eklendi. Kitap iade edildiğinde size bildirim gönderilecektir.");
            }
            catch (Exception ex)
            {
                PrintError("Rezervasyon hatası: " + ex.Message);
            }
            WaitUser();
        }

        private void StudentAddReview()
        {
            Console.Clear();
            PrintHeader("KİTAP PUANLA & YORUM YAP");
            Console.Write(" Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            Console.Write(" Puanınız (1-5): ");
            if (int.TryParse(Console.ReadLine(), out int rating))
            {
                Console.Write(" Yorumunuz: ");
                string comment = Console.ReadLine() ?? "";

                try
                {
                    libraryManager.AddReview(SessionManager.CurrentUser!.Username, id, rating, comment);
                    PrintSuccess("Değerlendirmeniz başarıyla eklenmiştir. Katkınız için teşekkürler!");
                }
                catch (Exception ex)
                {
                    PrintError("Değerlendirme hatası: " + ex.Message);
                }
            }
            else
            {
                PrintError("Geçersiz puan formatı!");
            }
            WaitUser();
        }

        private void StudentViewReviews()
        {
            Console.Clear();
            PrintHeader("KİTAP YORUMLARI");
            Console.Write(" Yorumlarını görmek istediğiniz Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            var book = libraryManager.Books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
            {
                PrintError("Kitap bulunamadı!");
                WaitUser();
                return;
            }

            var reviews = libraryManager.GetBookReviews(id);
            Console.WriteLine($"\n [Kitap: {book.BookName} (Yazar: {book.Author})] Yorumları:");
            Console.WriteLine(new string('-', 70));

            if (reviews.Count == 0)
            {
                PrintInfo("Bu kitap için henüz değerlendirme yapılmamış.");
            }
            else
            {
                foreach (var r in reviews)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($" [{r.Date:dd.MM.yyyy}] Üye: {r.Username}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($" | Puan: {new string('★', r.Rating)}");
                    Console.ResetColor();
                    Console.WriteLine($" Yorum: \"{r.Comment}\"");
                    Console.WriteLine(new string('-', 50));
                }
            }
            WaitUser();
        }

        private void StudentManageFavorite()
        {
            Console.Clear();
            PrintHeader("FAVORİLERE EKLE / ÇIKAR");
            Console.Write(" Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            var list = libraryManager.GetUserFavorites(SessionManager.CurrentUser!.Username);
            bool isFav = list.Any(b => b.BookID == id);

            if (isFav)
            {
                libraryManager.RemoveFromFavorites(SessionManager.CurrentUser!.Username, id);
                PrintSuccess("Kitap favorilerinizden başarıyla çıkarılmıştır.");
            }
            else
            {
                try
                {
                    if (libraryManager.AddToFavorites(SessionManager.CurrentUser!.Username, id))
                    {
                        PrintSuccess("Kitap favorilerinize eklendi.");
                    }
                    else
                    {
                        PrintError("Kitap eklenemedi veya zaten favoride.");
                    }
                }
                catch (Exception ex)
                {
                    PrintError("Hata: " + ex.Message);
                }
            }
            WaitUser();
        }

        private void StudentListFavorites()
        {
            Console.Clear();
            PrintHeader("FAVORİ KİTAP LİSTEM");
            var list = libraryManager.GetUserFavorites(SessionManager.CurrentUser!.Username);
            DrawBookTable(list);
            WaitUser();
        }

        private void StudentNotifications()
        {
            Console.Clear();
            PrintHeader("BİLDİRİM KUTUSU");
            var nots = libraryManager.GetUserNotifications(SessionManager.CurrentUser!.Username);

            if (nots.Count == 0)
            {
                PrintInfo("Herhangi bir bildiriminiz bulunmamaktadır.");
            }
            else
            {
                foreach (var n in nots)
                {
                    string readStatus = n.IsRead ? "[Okundu]" : "[YENİ]";
                    Console.WriteLine($" -> {readStatus} [{n.Date:dd.MM.yyyy HH:mm}] {n.Message}");
                }
                libraryManager.MarkAllNotificationsAsRead(SessionManager.CurrentUser!.Username);
            }
            WaitUser();
        }

        private void StudentPayPenalty()
        {
            Console.Clear();
            PrintHeader("GEÇİKME CEZASI ÖDE");
            var student = (Student)SessionManager.CurrentUser!;

            if (student.PenaltyBalance <= 0)
            {
                PrintInfo("Ödenmesi gereken herhangi bir ceza borcunuz bulunmamaktadır.");
                WaitUser();
                return;
            }

            Console.WriteLine($" Toplam Borç Tutarınız: {student.PenaltyBalance:C}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" Ödeme yapmak istiyor musunuz? (E/H): ");
            Console.ResetColor();

            string choice = Console.ReadLine() ?? "";
            if (choice.Equals("E", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    libraryManager.PayAllPenalties(student.Username);
                    PrintSuccess("Cezanız başarıyla ödenmiştir. Kitap alma engeliniz kalkmıştır.");
                }
                catch (Exception ex)
                {
                    PrintError("Ödeme esnasında hata: " + ex.Message);
                }
            }
            else
            {
                PrintInfo("Ödeme iptal edildi.");
            }
            WaitUser();
        }
        #endregion

        #region Yönetici Dashboard
        private void AdminDashboardLoop()
        {
            var admin = (Admin)SessionManager.CurrentUser!;

            while (true)
            {
                Console.Clear();
                PrintAppBanner();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" [YÖNETİCİ PANELİ] Admin: {admin.Name} ({admin.PermissionLevel})");
                Console.ResetColor();
                Console.WriteLine(new string('-', 70));

                // İstatistikleri Getir
                var stats = libraryManager.GetDashboardStatistics();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" Üye: {stats["TotalUsers"]} | ");
                Console.Write($" Kitap: {stats["TotalBooks"]} | ");
                Console.Write($" Ödünç: {stats["ActiveBorrows"]} | ");
                Console.WriteLine($" Geciken: {stats["OverdueBooks"]}");
                Console.ResetColor();
                Console.WriteLine(new string('-', 70));

                PrintHeader("YÖNETİM MENÜSÜ");
                Console.WriteLine(" [1] Yeni Kitap Ekle");
                Console.WriteLine(" [2] Kitap Bilgisi Güncelle");
                Console.WriteLine(" [3] Kitap Sil");
                Console.WriteLine(" [4] Kitap Katalog Arama");
                Console.WriteLine(" [5] Kayıtlı Üyeleri Listele");
                Console.WriteLine(" [6] Cezaları Yönet (dusen.txt)");
                Console.WriteLine(" [7] Sistem Loglarını İzle (kayıtlar.txt)");
                Console.WriteLine(" [8] Oturumu Kapat");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" Seçiminiz (1-8): ");
                Console.ResetColor();

                string opt = Console.ReadLine() ?? "";
                if (opt == "1") AdminAddBook();
                else if (opt == "2") AdminUpdateBook();
                else if (opt == "3") AdminDeleteBook();
                else if (opt == "4") AdminSearchBooks();
                else if (opt == "5") AdminListUsers();
                else if (opt == "6") AdminManagePenalties();
                else if (opt == "7") AdminViewLogs();
                else if (opt == "8")
                {
                    loggerService.WriteLog($"{admin.Name} oturumu kapattı.");
                    SessionManager.ClearSession();
                    PrintInfo("Oturum kapatıldı.");
                    WaitUser();
                    break;
                }
                else
                {
                    PrintError("Geçersiz seçim!");
                    WaitUser();
                }
            }
        }

        private void AdminAddBook()
        {
            Console.Clear();
            PrintHeader("YENİ KİTAP EKLE");

            Console.Write(" Kitap ID (Örn: BK-105): ");
            string id = Console.ReadLine() ?? "";

            Console.Write(" Kitap Adı: ");
            string name = Console.ReadLine() ?? "";

            Console.Write(" Yazar: ");
            string author = Console.ReadLine() ?? "";

            Console.Write(" Kategori: ");
            string category = Console.ReadLine() ?? "";

            Console.Write(" Sayfa Sayısı: ");
            int.TryParse(Console.ReadLine(), out int pages);

            Console.Write(" Raf Numarası: ");
            string shelf = Console.ReadLine() ?? "";

            Console.Write(" Stok Sayısı: ");
            int.TryParse(Console.ReadLine(), out int stock);

            try
            {
                libraryManager.AddBook(id, name, author, category, pages, shelf, stock);
                PrintSuccess("Kitap başarıyla kataloğa eklendi!");
            }
            catch (Exception ex)
            {
                PrintError("Ekleme başarısız: " + ex.Message);
            }
            WaitUser();
        }

        private void AdminUpdateBook()
        {
            Console.Clear();
            PrintHeader("KİTAP BİLGİSİ GÜNCELLE");
            Console.Write(" Güncellemek istediğiniz Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            var book = libraryManager.Books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
            {
                PrintError("Kitap bulunamadı!");
                WaitUser();
                return;
            }

            Console.WriteLine($"\n Mevcut Bilgiler: {book.BookName} - {book.Author} | Kategori: {book.Category} | Raf: {book.ShelfNumber} | Stok: {book.Stock}");
            Console.WriteLine(" (Değiştirmek istemediğiniz alanları boş geçebilirsiniz.)\n");

            Console.Write($" Yeni Kitap Adı ({book.BookName}): ");
            string nameInput = Console.ReadLine() ?? "";
            string name = string.IsNullOrWhiteSpace(nameInput) ? book.BookName : nameInput;

            Console.Write($" Yeni Yazar ({book.Author}): ");
            string authorInput = Console.ReadLine() ?? "";
            string author = string.IsNullOrWhiteSpace(authorInput) ? book.Author : authorInput;

            Console.Write($" Yeni Kategori ({book.Category}): ");
            string catInput = Console.ReadLine() ?? "";
            string category = string.IsNullOrWhiteSpace(catInput) ? book.Category : catInput;

            Console.Write($" Yeni Sayfa Sayısı ({book.PageCount}): ");
            string pageInput = Console.ReadLine() ?? "";
            int pageCount = string.IsNullOrWhiteSpace(pageInput) ? book.PageCount : int.Parse(pageInput);

            Console.Write($" Yeni Raf Numarası ({book.ShelfNumber}): ");
            string shelfInput = Console.ReadLine() ?? "";
            string shelf = string.IsNullOrWhiteSpace(shelfInput) ? book.ShelfNumber : shelfInput;

            Console.Write($" Yeni Stok Miktarı ({book.Stock}): ");
            string stockInput = Console.ReadLine() ?? "";
            int stock = string.IsNullOrWhiteSpace(stockInput) ? book.Stock : int.Parse(stockInput);

            try
            {
                libraryManager.UpdateBook(id, name, author, category, pageCount, shelf, stock);
                PrintSuccess("Kitap başarıyla güncellenmiştir.");
            }
            catch (Exception ex)
            {
                PrintError("Güncelleme hatası: " + ex.Message);
            }
            WaitUser();
        }

        private void AdminDeleteBook()
        {
            Console.Clear();
            PrintHeader("KİTAP SİL");
            Console.Write(" Silmek istediğiniz Kitap ID girin: ");
            string id = Console.ReadLine() ?? "";

            var book = libraryManager.Books.FirstOrDefault(b => b.BookID == id);
            if (book == null)
            {
                PrintError("Kitap bulunamadı!");
                WaitUser();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" '{book.BookName}' kitabını silmek istediğinize emin misiniz? (E/H): ");
            Console.ResetColor();

            string choice = Console.ReadLine() ?? "";
            if (choice.Equals("E", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    libraryManager.DeleteBook(id);
                    PrintSuccess("Kitap katalogdan silindi.");
                }
                catch (Exception ex)
                {
                    PrintError("Silme hatası: " + ex.Message);
                }
            }
            else
            {
                PrintInfo("Silme işlemi iptal edildi.");
            }
            WaitUser();
        }

        private void AdminSearchBooks()
        {
            Console.Clear();
            PrintHeader("KİTAP ARAMA & KATALOG");
            Console.Write(" Arama kelimesi girin: ");
            string query = Console.ReadLine() ?? "";

            var books = libraryManager.SearchBooks(query);
            DrawBookTable(books);
            WaitUser();
        }

        private void AdminListUsers()
        {
            Console.Clear();
            PrintHeader("SİSTEM KAYITLI ÜYELERİ");

            Console.WriteLine(string.Format(" {0,-15} | {1,-12} | {2,-10} | {3,-15} | {4,-20} | {5,-10}", "Ad Soyad", "Kullanıcı Adı", "Rol", "Telefon", "E-Posta", "Ceza Bakiye"));
            Console.WriteLine(new string('-', 95));

            foreach (var u in libraryManager.Users)
            {
                string role = u is Admin ? "Yönetici" : "Öğrenci";
                string balance = u is Student s ? s.PenaltyBalance.ToString("C", CultureInfo.GetCultureInfo("tr-TR")) : "-";
                Console.WriteLine(string.Format(" {0,-15} | {1,-12} | {2,-10} | {3,-15} | {4,-20} | {5,-10}", 
                    u.Name.Length > 15 ? u.Name.Substring(0, 12) + "..." : u.Name, 
                    u.Username, 
                    role, 
                    u.Phone, 
                    u.Email.Length > 20 ? u.Email.Substring(0, 17) + "..." : u.Email, 
                    balance));
            }
            Console.WriteLine(new string('-', 95));
            WaitUser();
        }

        private void AdminManagePenalties()
        {
            Console.Clear();
            PrintHeader("CEZA YÖNETİMİ (dusen.txt)");

            var activePenalties = libraryManager.Penalties.Where(p => !p.IsPaid).ToList();
            if (activePenalties.Count == 0)
            {
                PrintInfo("Ödenmemiş herhangi bir ceza kaydı bulunmamaktadır.");
                WaitUser();
                return;
            }

            Console.WriteLine(string.Format(" {0,-10} | {1,-15} | {2,-10} | {3,-10} | {4,-12}", "Ceza ID", "Kullanıcı Adı", "Kitap ID", "Tutar", "Tarih"));
            Console.WriteLine(new string('-', 65));

            foreach (var p in activePenalties)
            {
                Console.WriteLine(string.Format(" {0,-10} | {1,-15} | {2,-10} | {3,-10} | {4,-12}", 
                    p.PenaltyID, p.Username, p.BookID, p.Amount.ToString("C"), p.AccruedDate.ToString("dd.MM.yyyy")));
            }
            Console.WriteLine(new string('-', 65));

            Console.Write("\n Ödeme onayı vermek istediğiniz Ceza ID girin: ");
            string pId = Console.ReadLine() ?? "";

            var pen = activePenalties.FirstOrDefault(x => x.PenaltyID == pId);
            if (pen == null)
            {
                PrintError("Geçersiz Ceza ID!");
                WaitUser();
                return;
            }

            try
            {
                libraryManager.PayPenalty(pen.Username, pId);
                PrintSuccess("Ceza elden ödeme kaydı işlendi ve silindi.");
            }
            catch (Exception ex)
            {
                PrintError("İşlem hatası: " + ex.Message);
            }
            WaitUser();
        }

        private void AdminViewLogs()
        {
            Console.Clear();
            PrintHeader("SİSTEM GÜNLÜK LOGLARI (kayıtlar.txt)");
            var logs = loggerService.ReadAllLogs().Reverse().Take(30).ToList();

            if (logs.Count == 0)
            {
                PrintInfo("Herhangi bir log kaydı bulunmamaktadır.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (var log in logs)
                {
                    Console.WriteLine(log);
                }
                Console.ResetColor();
            }
            WaitUser();
        }
        #endregion

        #region Konsol Yardımcı Metotları
        private void PrintAppBanner()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"  ___   _  __   __ ___  _      _    ___   ");
            Console.WriteLine(@" |   \ /_\ \ \ / /|_ _|| |    /_\  | _ \  ");
            Console.WriteLine(@" | |) / _ \ \ V /  | | | |__ / _ \ |   /  ");
            Console.WriteLine(@" |___/_/ \_\ |_|  |___||____/_/ \_\|_|_\  ");
            Console.WriteLine(@"                                          ");
            Console.WriteLine(@"  _  __ _   _  _____  _   _  ___  _  _    _   _  _  ___  ___  ___  ");
            Console.WriteLine(@" | |/ /| | | ||_   _|| | | || _ \| || |  /_\ | \| || __|/ __||_ _| ");
            Console.WriteLine(@" | ' < | |_| |  | |  | |_| ||  _/| __ | / _ \| .` || _| \__ \ | |  ");
            Console.WriteLine(@" |_|\_\ \___/   |_|   \___/ |_|  |_||_|/_/ \_\_|\_||___||___/|___| ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("    Gelişmiş Kütüphane Otomasyon Sistemi | Nesne Tabanlı Programlama Projesi");
            Console.WriteLine(new string('=', 78));
            Console.ResetColor();
            Console.WriteLine();
        }

        private void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" === {title} === ");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n ✓ BAŞARILI: {message}");
            Console.ResetColor();
        }

        private void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n ✗ HATA: {message}");
            Console.ResetColor();
        }

        private void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n i BİLGİ: {message}");
            Console.ResetColor();
        }

        private void WaitUser()
        {
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey(true);
        }

        private void DrawBookTable(List<Book> books)
        {
            if (books.Count == 0)
            {
                PrintInfo("Eşleşen herhangi bir kitap bulunamadı.");
                return;
            }

            Console.WriteLine(string.Format(" {0,-10} | {1,-22} | {2,-15} | {3,-15} | {4,-6} | {5,-6} | {6,-5}", 
                "Kitap ID", "Kitap Adı", "Yazar", "Kategori", "Raf", "Stok", "Puan"));
            Console.WriteLine(new string('-', 92));

            foreach (var b in books)
            {
                string name = b.BookName.Length > 22 ? b.BookName.Substring(0, 19) + "..." : b.BookName;
                string author = b.Author.Length > 15 ? b.Author.Substring(0, 12) + "..." : b.Author;
                string category = b.Category.Length > 15 ? b.Category.Substring(0, 12) + "..." : b.Category;

                Console.WriteLine(string.Format(" {0,-10} | {1,-22} | {2,-15} | {3,-15} | {4,-6} | {5,-6} | {6,-5:0.0}", 
                    b.BookID, name, author, category, b.ShelfNumber, b.Stock, b.AverageRating));
            }
            Console.WriteLine(new string('-', 92));
        }
        #endregion
    }
}
