namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kullanıcıların favori kitap kayıtlarını temsil eden sınıf.
    /// </summary>
    public class FavoriteRecord
    {
        public string Username { get; set; }
        public string BookID { get; set; }

        public FavoriteRecord(string username, string bookId)
        {
            Username = username;
            BookID = bookId;
        }
    }
}
