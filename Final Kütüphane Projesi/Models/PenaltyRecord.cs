using System;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Geciken kitaplar dolayısıyla düşen ceza kayıtlarını temsil eden sınıf.
    /// </summary>
    public class PenaltyRecord
    {
        public string PenaltyID { get; set; }
        public string Username { get; set; }
        public string BookID { get; set; }
        public double Amount { get; set; }
        public DateTime AccruedDate { get; set; }
        public bool IsPaid { get; set; }

        public PenaltyRecord(string penaltyId, string username, string bookId, double amount, DateTime accruedDate, bool isPaid = false)
        {
            PenaltyID = penaltyId;
            Username = username;
            BookID = bookId;
            Amount = amount;
            AccruedDate = accruedDate;
            IsPaid = isPaid;
        }
    }
}
