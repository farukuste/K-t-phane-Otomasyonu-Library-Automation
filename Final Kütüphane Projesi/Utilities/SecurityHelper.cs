using System.Security.Cryptography;
using System.Text;

namespace Final_Kütüphane_Projesi.Utilities
{
    /// <summary>
    /// Şifreleme ve güvenlik işlemlerini gerçekleştiren yardımcı sınıf.
    /// (OOP: Static Yapı Kullanımı)
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Verilen düz metni SHA256 algoritmasıyla şifreler.
        /// </summary>
        /// <param name="rawData">Düz metin şifre</param>
        /// <returns>64 karakter uzunluğunda hex formatında şifrelenmiş metin</returns>
        public static string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrEmpty(rawData)) 
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
