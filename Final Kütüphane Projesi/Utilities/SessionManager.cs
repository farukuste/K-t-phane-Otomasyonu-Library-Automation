using Final_Kütüphane_Projesi.Models;

namespace Final_Kütüphane_Projesi.Utilities
{
    /// <summary>
    /// Giriş yapan kullanıcının bilgilerini ve oturum durumunu takip eden sınıf.
    /// (OOP: Static Yapı Kullanımı)
    /// </summary>
    public static class SessionManager
    {
        /// <summary>
        /// Giriş yapmış olan mevcut kullanıcı (Öğrenci veya Yönetici olabilir).
        /// </summary>
        public static Person? CurrentUser { get; private set; }

        /// <summary>
        /// Kullanıcının giriş yapıp yapmadığını belirtir.
        /// </summary>
        public static bool IsLoggedIn => CurrentUser != null;

        /// <summary>
        /// Giriş yapan kullanıcının Admin olup olmadığını belirtir.
        /// </summary>
        public static bool IsAdmin => CurrentUser is Admin;

        /// <summary>
        /// Giriş yapan kullanıcının Öğrenci olup olmadığını belirtir.
        /// </summary>
        public static bool IsStudent => CurrentUser is Student;

        /// <summary>
        /// Oturumu başlatır.
        /// </summary>
        public static void SetSession(Person user)
        {
            CurrentUser = user;
        }

        /// <summary>
        /// Oturumu sonlandırır.
        /// </summary>
        public static void ClearSession()
        {
            CurrentUser?.Logout();
            CurrentUser = null;
        }
    }
}
