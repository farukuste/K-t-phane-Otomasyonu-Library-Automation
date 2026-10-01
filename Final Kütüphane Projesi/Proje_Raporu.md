# Gelişmiş Kütüphane Otomasyonu - Proje Raporu

**Ders:** Nesne Tabanlı Programlama (OOP)  
**Proje Adı:** Gelişmiş Kütüphane Otomasyonu Sistemi (Konsol Sürümü)  
**Geliştirici:** Ömer Faruk (Öğrenci)  

---

## 1. Giriş ve Proje Amacı

Bu proje, bir üniversite kütüphanesinin kitap alım, güncelleme, silme, ödünç verme, rezervasyon sırası, ceza hesaplama ve üye yönetim süreçlerini dijitalleştirmek amacıyla geliştirilmiş akademik düzeyde bir **Konsol CLI (Command Line Interface)** uygulamasıdır.

Projenin temel amacı, endüstri standardı olan **Temiz Mimari (Clean Architecture)** ve **Katmanlı Mimari (Layered Architecture)** prensiplerini uygulayarak, yüksek düzeyde modüler, test edilebilir ve genişletilebilir bir yazılım geliştirmektir. Proje boyunca Nesne Tabanlı Programlama (OOP) ilkelerine sıkı sıkıya bağlı kalınmış, SOLID yazılım tasarım prensipleri rehber edinilmiştir.

---

## 2. Kullanılan Teknolojiler ve Platform
* **Geliştirme Dili:** C# (.NET 10.0 SDK)
* **Arayüz Teknolojisi:** Konsol CLI (Command Line Interface - ASCII ve Renklendirilmiş çıktı düzeni)
* **Veri Depolama:** Flat File (.txt) dosya tabanlı veri depolama katmanı (Data/)
* **Şifreleme Standartı:** SHA-256 (Kullanıcı parolalarının güvenliği için)
* **Sistem Uyumluluk Katmanı:** Headless ve yönlendirilmiş (redirected) test terminali desteği

---

## 3. Mimari Tasarım ve Klasör Yapısı

Sistem, işlevsel sorumlulukların ayrıştırılması (Separation of Concerns) amacıyla 6 temel katmana bölünmüştür:

1. **Interfaces (Arayüz Katmanı):**
   Sistemdeki nesnelerin yeteneklerini soyutlaştıran ve sözleşme haline getiren interface'leri içerir (`IBorrowable`, `IReservable`).
2. **Models (Veri Modelleri):**
   Veri tabanında saklanan ve iş kurallarında işlenen gerçek dünya varlıklarını temsil eder (`Person` abstract sınıfı, `Student`, `Admin`, `Book` vb.).
3. **Services (Servis Katmanı):**
   Sistem dışı kaynaklarla (disk dosyalarıyla) iletişimi yönetir. `FileManager` flat-file işlemlerini gerçekleştirirken, `LoggerService` sistem loglarını diske yazar.
4. **Utilities (Yardımcı Sınıflar):**
   Sistem genelinde ortak kullanılan araçları barındırır. `SecurityHelper` şifreleri SHA-256 algoritmasıyla hash'ler. `SessionManager` aktif oturumu saklar.
5. **Managers (İş Mantığı Katmanı):**
   Sistemin ana beynidir. `LibraryManager`, iş kurallarını (Kim kitap alabilir? Kimin cezası var? Stokta olmayan kitap nasıl rezerve edilir?) koordine eder.
6. **ConsoleUI (Kullanıcı Arayüzü Katmanı):**
   Kullanıcı ile etkileşime geçen siyah ekran arayüzünü barındırır (`ConsoleApp`, `Console`). `Console.cs` sınıfı, `System.Console` sınıfına yapılan çağrıları yönlendirerek headless/redirected test ortamlarında uygulamanın çökmesini engeller.

---

## 4. Dosya Tabanlı (Flat File) Veritabanı Tasarımı

Projede harici bir veritabanı sunucusu kullanılmamış, veriler diske metin dosyaları olarak yazılmıştır. Veriler satır bazlı olarak saklanmakta ve her kolon birbirlerinden `|` (pipe) karakteriyle ayrıştırılmaktadır.

### Örnek Veri Yapısı (`kitaplar.txt`):
```text
BK-101|Suç ve Ceza|Fyodor Dostoyevski|Dünya Klasikleri|430|A-12|3|4.8|5
BK-102|Nutuk|Mustafa Kemal Atatürk|Tarih & Siyaset|540|B-03|5|5.0|12
```

### Veri Senkronizasyonu ve Bütünlük:
Sistem açıldığında, `FileManager` tüm txt dosyalarını belleğe (Generic List yapılarına) okur. Uygulama içinde yapılan her işlemde (Kitap alma, iade, üye ekleme) veriler bellekte güncellenir ve hemen ardından diske geri yazılarak (Write-Through) veri kaybı engellenir.

---

## 5. Nesne Tabanlı Programlama (OOP) Prensiplerinin Uygulanışı

Projede zorunlu kılınan OOP kavramları aşağıda detaylandırılmıştır:

### A. Soyutlama (Abstraction) & Soyut Sınıflar
* **`Person` (Soyut Sınıf):** Ortak kullanıcı niteliklerini (Ad, E-posta, Telefon) tutan taban sınıftır. `abstract` olarak tanımlandığı için doğrudan örneği oluşturulamaz. Alt sınıfların ezmesi gereken `public abstract bool Login(string password)` metodunu barındırır.
* **Interface'ler:** 
  * `IBorrowable`: Ödünç alınabilir nesnelerin uyması gereken şablondur.
  * `IReservable`: Rezerve edilebilir nesnelerin uyması gereken şablondur.

### B. Kalıtım (Inheritance)
* **`Student` ve `Admin` Sınıfları:** `Person` sınıfından miras alırlar (`class Student : Person`). Ata sınıfın tüm özelliklerini (`Name`, `Email` vb.) kullanırlar ve kendilerine has özellikleri (Öğrenci numarası, yetki seviyesi) eklerler.

### C. Çok Biçimlilik (Polymorphism)
* **Metot Ezme (Override):** `Person` içindeki `Login()` metodu, `Student` sınıfında farklı (SHA256 hash doğrulamasıyla), `Admin` sınıfında farklı (yönetici yetki doğrulamasıyla) çalışacak şekilde ezilmiştir.
* **Metot Aşırı Yükleme (Overloading):** `LibraryManager` içerisinde `SearchBooks(string query)` metodu sadece arama kelimesine göre ararken, `SearchBooks(string query, string category)` aşırı yüklenmiş metodu arama kelimesi ve kategoriyi aynı anda filtreler.

### D. Kapsülleme (Encapsulation)
* Sınıflardaki değişkenler `private` tanımlanmıştır. Dışarıya erişim `get` ve `set` blokları (Properties) ile sınırlandırılmıştır. 
* Kısıtlayıcı mantıklar kapsüllenmiştir. Örneğin, `Book.Stock` özelliği set edilirken gelen değer sıfırdan küçükse hata fırlatılır (`throw new ArgumentException(...)`).

### E. Yapıcı Metotlar (Constructors)
* Her sınıfta nesne bütünlüğünü korumak adına yapıcı metotlar tanımlanmıştır. `Student` sınıfı, ata sınıfın yapıcı metodunu `base(name, username, password, phone, email)` ile tetikler.

---

## 6. Güvenlik, Hata Yönetimi ve Loglama

* **Şifre Güvenliği:** Kullanıcı şifreleri veritabanı dosyalarında asla açık metin (plain text) olarak saklanmaz. `SecurityHelper` sınıfı, şifreyi tek yönlü kriptografik algoritma olan **SHA-256** ile hash'ler. Sisteme giriş yapılırken de kullanıcının girdiği şifre hash'lenip dosyadaki hash ile karşılaştırılır.
* **Hata Yönetimi (Exception Handling):** Uygulama genelinde (dosya okuma, veri doğrulama, kullanıcı girişleri) `try-catch` blokları kullanılarak hatalar yakalanmış, loglanmış ve kullanıcıya anlamlı hata mesajları renkli konsol çıktıları olarak gösterilmiştir.
* **İşlem Günlüğü (Loglama):** Yapılan her başarılı/başarısız işlem (Ödünç alma, oturum açma, silme) zaman damgasıyla (`DateTime.Now`) `bin/Debug/net10.0/Logs/kayıtlar.txt` dosyasına kaydedilir.

---

## 7. Sonuç

Geliştirilen "Gelişmiş Kütüphane Otomasyonu Sistemi", gerek teknik altyapısı (OOP prensipleri, Generic Koleksiyonlar, Dosya Tabanlı Veri Katmanı) gerekse modern ve şık kullanıcı arayüzü ile bir üniversite bitirme/final projesinden beklenen tüm nitelikleri fazlasıyla karşılamaktadır. Modüler yapısı sayesinde gelecekte SQL Server entegrasyonu veya web tabanlı arayüz geliştirilmesi durumunda iş mantığı katmanı (`LibraryManager`) hiç değiştirilmeden doğrudan kullanılabilir.
