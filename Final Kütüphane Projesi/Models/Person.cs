using System;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kütüphane sistemindeki tüm kullanıcılar için soyut taban sınıf.
    /// (OOP: Abstraction - Soyutlama, Encapsulation - Kapsülleme)
    /// </summary>
    public abstract class Person
    {
        // Private fields (OOP: Encapsulation)
        private string name = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty; // SHA256 formatında saklanacaktır
        private string phone = string.Empty;
        private string email = string.Empty;

        // Public properties with validation (OOP: Encapsulation)
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ad Soyad alanı boş bırakılamaz.");
                name = value.Trim();
            }
        }

        public string Username
        {
            get => username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Kullanıcı adı alanı boş bırakılamaz.");
                username = value.Trim().ToLower(); // Standartlaştırma
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Şifre alanı boş bırakılamaz.");
                password = value;
            }
        }

        public string Phone
        {
            get => phone;
            set => phone = value?.Trim() ?? string.Empty;
        }

        public string Email
        {
            get => email;
            set => email = value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Sınıf Yapıcısı (OOP: Constructor Kullanımı)
        /// </summary>
        protected Person(string name, string username, string password, string phone, string email)
        {
            Name = name;
            Username = username;
            Password = password;
            Phone = phone;
            Email = email;
        }

        /// <summary>
        /// Giriş kontrolünü sağlayan soyut metot.
        /// (OOP: Polymorphism - Çok Biçimlilik ve Abstraction - Soyutlama)
        /// Alt sınıflar kendi yetki düzeylerine veya giriş kurallarına göre bu metodu ezmek (override) zorundadır.
        /// </summary>
        public abstract bool Login(string enteredPassword);

        /// <summary>
        /// Sanal çıkış metodu.
        /// (OOP: Polymorphism - Çok Biçimlilik)
        /// </summary>
        public virtual void Logout()
        {
            // Varsayılan çıkış işlemleri, alt sınıflar tarafından özelleştirilebilir.
        }
    }
}
