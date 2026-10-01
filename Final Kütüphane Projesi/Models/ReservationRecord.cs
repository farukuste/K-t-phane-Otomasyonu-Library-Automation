using System;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kitap rezervasyon kayıtlarını temsil eden sınıf.
    /// </summary>
    public class ReservationRecord
    {
        public string ReservationID { get; set; }
        public string Username { get; set; }
        public string BookID { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsActive { get; set; }

        public ReservationRecord(string reservationId, string username, string bookId, DateTime reservationDate, bool isActive = true)
        {
            ReservationID = reservationId;
            Username = username;
            BookID = bookId;
            ReservationDate = reservationDate;
            IsActive = isActive;
        }
    }
}
