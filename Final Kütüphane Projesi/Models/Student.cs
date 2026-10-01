using System;
using System.Collections.Generic;
using Final_Kütüphane_Projesi.Utilities;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Öğrenci kullanıcısını temsil eden sınıf.
    /// (OOP: Inheritance - Kalıtım)
    /// </summary>
    public class Student : Person
    {
        private string studentNumber = string.Empty;
        private List<string> borrowedBooks = new List<string>(); // OOP: Generic List kullanımı

        public string StudentNumber
        {
            get => studentNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Öğrenci numarası boş bırakılamaz.");
                studentNumber = value.Trim();
            }
        }

        public List<string> BorrowedBooks
        {
            get => borrowedBooks;
            set => borrowedBooks = value ?? new List<string>();
        }

        /// <summary>
        /// Öğrencinin toplam ceza borcu.
        /// </summary>
        public double PenaltyBalance { get; set; }

        /// <summary>
        /// Öğrenci sınıfı yapıcısı.
        /// </summary>
        public Student(string name, string username, string password, string studentNumber, string phone, string email, double penaltyBalance = 0.0)
            : base(name, username, password, phone, email) // OOP: Base sınıf yapıcısının tetiklenmesi
        {
            StudentNumber = studentNumber;
            BorrowedBooks = new List<string>();
            PenaltyBalance = penaltyBalance;
        }

        /// <summary>
        /// Giriş metodunun ezilmesi.
        /// (OOP: Polymorphism - Çok Biçimlilik)
        /// </summary>
        public override bool Login(string enteredPassword)
        {
            // Şifre SHA256 ile hash'lenerek karşılaştırılır.
            string hashedInput = SecurityHelper.ComputeSha256Hash(enteredPassword);
            return this.Password.Equals(hashedInput, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Çıkış işlemini özelleştirir.
        /// (OOP: Polymorphism - Sanal Metot Ezme)
        /// </summary>
        public override void Logout()
        {
            base.Logout();
            // Öğrenci çıkış yapınca yapılacak ek işlemler (gerekirse loglama vb.)
        }
    }
}
