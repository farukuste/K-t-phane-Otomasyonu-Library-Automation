using System;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kitap ödünç alma kayıtlarını temsil eden sınıf.
    /// </summary>
    public class BorrowRecord
    {
        public string BorrowID { get; set; }
        public string Username { get; set; }
        public string BookID { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;
        public double PenaltyAmount { get; set; }
        public bool PenaltyPaid { get; set; }

        public BorrowRecord(string borrowId, string username, string bookId, DateTime borrowDate, DateTime dueDate, DateTime? returnDate, double penaltyAmount = 0.0, bool penaltyPaid = false)
        {
            BorrowID = borrowId;
            Username = username;
            BookID = bookId;
            BorrowDate = borrowDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
            PenaltyAmount = penaltyAmount;
            PenaltyPaid = penaltyPaid;
        }
    }
}
