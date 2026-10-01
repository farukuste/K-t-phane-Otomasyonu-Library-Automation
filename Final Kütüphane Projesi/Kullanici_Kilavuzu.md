# Gelişmiş Kütüphane Otomasyonu - Kullanım Kılavuzu (Konsol Paneli)

Bu kılavuz, tamamen yenilenen siyah ekran konsol arayüzümüzü (CLI) kullanacak olan öğrenciler ve yöneticiler için ekran akışını adım adım açıklamaktadır.

---

## 🔑 Ana Menü, Giriş ve Kayıt Ekranı

Uygulamayı çalıştırdığınızda (`dotnet run`), karşınıza üst kısımda ASCII art ile yazılmış bir kütüphane afişi ve 3 seçenekli ana giriş menüsü gelir:

### 1. Giriş Yap (Ana Menü - Seçenek 1)
* **Kullanıcı Adı** bilginizi girin ve Enter'a basın.
* **Şifre** bilginizi girerken, karakterler ekranda güvenlik amacıyla yıldız (`*`) olarak maskelenir ve gizlenir.
* Doğru kullanıcı adı ve şifre girdiğinizde sistem otomatik olarak yetki seviyenizi algılayarak sizi ilgili panele yönlendirir:
  * **Yönetici Girişi:** Kullanıcı Adı: `admin` | Şifre: `admin123`
  * **Öğrenci Girişi:** Kullanıcı Adı: `ahmet` | Şifre: `ahmet123`

### 2. Öğrenci Kayıt Ol (Ana Menü - Seçenek 2)
* Sırasıyla Ad-Soyad, benzersiz bir Kullanıcı Adı, Şifre (yazarken maskelenir), Öğrenci Numarası, Telefon Numarası ve E-Posta bilgilerini girmeniz istenir.
* Tüm bilgileri girip tamamladığınızda öğrenci kaydı başarıyla oluşturulur ve ana menüye dönülür.

---

## 🎓 Öğrenci Paneli Kullanımı (Student Dashboard)

Giriş yaptıktan sonra, üst tarafta öğrencinin adı, numarası, varsa **gecikme ceza borcu** ve elindeki aktif kitap sayısı dinamik olarak gösterilir.

Öğrenci menüsünde aşağıdaki 11 seçenek yer alır:

1. **Kitap Ara & Katalog Listele:** Arama terimi (Kitap adı, yazar vb.) ve isteğe bağlı kategori girerek arama yapabilirsiniz. Arama sonrasında kitapları stoktakiler, en çok okunanlar veya yeni eklenenlere göre süzebilirsiniz.
2. **Kitap Ödünç Al:** İstediğiniz kitabın benzersiz kodunu (Örn: `BK-101`) girerek kitabı 14 günlüğüne ödünç alabilirsiniz. (Not: Ceza borcunuz varsa veya limitinizi aşmışsanız hata alırsınız).
3. **Kitap İade Et (Teslim Et):** Şu an elinizde bulunan kitapları listeler. İade etmek istediğiniz Kitap ID'sini girerek kütüphaneye teslim edebilirsiniz. Teslim tarihi geçmişse sistem ceza borcu yansıtır.
4. **Kitap Rezerve Et:** Stokta kalmayan (stok adedi 0 olan) bir kitap için rezervasyon yapmanızı sağlar. Kitap kütüphaneye iade edildiği an size otomatik bildirim gelir.
5. **Kitap Puanla & Yorum Yap:** Okuduğunuz kitaplara 1-5 arası yıldız puanı verebilir ve yazılı görüşlerinizi ekleyebilirsiniz.
6. **Yorumları ve Değerlendirmeleri Gör:** Bir Kitap ID girerek o kitaba diğer üyeler tarafından yapılmış puan ve yorumları listeler.
7. **Kitap Favorilere Ekle / Kaldır:** Sık takip ettiğiniz kitapları favori listenize ekleyebilir veya çıkarabilirsiniz.
8. **Favori Kitaplarımı Listele:** Favorilerinize eklediğiniz kitapları tablo biçiminde gösterir.
9. **Bildirim Kutusunu Aç:** Rezerve kitap durumları veya ceza durumları gibi size gönderilen bildirim geçmişini tarih sırasına göre okumanızı sağlar.
10. **Gecikme Cezası Öde:** Ceza borcunuz bulunuyorsa bu menüden borcunuzu ödeyip kütüphaneyi aktif kullanmaya devam edebilirsiniz.
11. **Oturumu Kapat:** Oturumunuzu kapatıp ana giriş menüsüne geri döner.

---

## 👤 Yönetici Paneli Kullanımı (Admin Dashboard)

Yöneticiler sisteme girdiğinde üstte anlık istatistik kartları (Toplam Üye, Toplam Kitap Tipi, Aktif Ödünç ve Geciken Kitap sayısı) gösterilir.

Yönetici menüsünde aşağıdaki 8 seçenek yer alır:

1. **Yeni Kitap Ekle:** Kütüphane kataloğuna yeni bir kitap ekler. Kitap ID, Adı, Yazar, Kategori, Sayfa Sayısı, Raf No ve Başlangıç Stoku girilir.
2. **Kitap Bilgisi Güncelle:** Güncellemek istediğiniz Kitap ID'sini girdikten sonra güncel bilgileri girin. Değiştirmek istemediğiniz bilgileri doğrudan Enter ile boş geçebilirsiniz.
3. **Kitap Sil:** Kitap ID girerek kitabı katalogdan tamamen siler. (Eğer kitap bir öğrencide ödünçte ise silmeye izin verilmez).
4. **Kitap Katalog Arama:** Tüm kitapları arama kelimesine göre listeler.
5. **Kayıtlı Üyeleri Listele:** Sistemdeki tüm yöneticileri ve öğrencileri, iletişim ve borç bakiyeleri ile birlikte tablo halinde listeler.
6. **Cezaları Yönet (dusen.txt):** Ödenmemiş tüm ceza kayıtlarını gösterir. Öğrenci elden ceza ödemesi yaptığında, ilgili Ceza ID'sini girerek cezayı sistemden silebilirsiniz.
7. **Sistem Loglarını İzle (kayıtlar.txt):** Kütüphanede yapılan son 30 sistem hareketini en yeni tarihten en eskiye doğru konsolda listeler.
8. **Oturumu Kapat:** Yönetici oturumunu güvenli bir şekilde kapatır ve ana menüye döner.
