using System;
using Final_Kütüphane_Projesi.Utilities;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Yönetici (Admin) kullanıcısını temsil eden sınıf.
    /// (OOP: Inheritance - Kalıtım)
    /// </summary>
    public class Admin : Person
    {
        private string permissionLevel = "Standard";

        public string PermissionLevel
        {
            get => permissionLevel;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    permissionLevel = "Standard";
                else
                    permissionLevel = value.Trim();
            }
        }

        /// <summary>
        /// Admin sınıfı yapıcısı.
        /// </summary>
        public Admin(string name, string username, string password, string permissionLevel, string phone, string email)
            : base(name, username, password, phone, email) // OOP: Base sınıf yapıcısının tetiklenmesi
        {
            PermissionLevel = permissionLevel;
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
    }
}
