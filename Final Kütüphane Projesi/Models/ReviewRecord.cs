using System;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kitaplara yapılan yorum ve puanlama kayıtlarını temsil eden sınıf.
    /// </summary>
    public class ReviewRecord
    {
        private int rating;

        public string ReviewID { get; set; }
        public string Username { get; set; }
        public string BookID { get; set; }

        public int Rating
        {
            get => rating;
            set
            {
                if (value < 1 || value > 5)
                    throw new ArgumentOutOfRangeException("Puan 1 ile 5 arasında olmalıdır.");
                rating = value;
            }
        }

        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public ReviewRecord(string reviewId, string username, string bookId, int rating, string comment, DateTime date)
        {
            ReviewID = reviewId;
            Username = username;
            BookID = bookId;
            Rating = rating;
            Comment = comment;
            Date = date;
        }
    }
}
