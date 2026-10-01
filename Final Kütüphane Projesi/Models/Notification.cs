using System;

namespace Final_Kütüphane_Projesi.Models
{
    /// <summary>
    /// Kullanıcılara gönderilen bildirim kayıtlarını temsil eden sınıf.
    /// </summary>
    public class Notification
    {
        public string NotificationID { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }

        public Notification(string notificationId, string username, string message, DateTime date, bool isRead = false)
        {
            NotificationID = notificationId;
            Username = username;
            Message = message;
            Date = date;
            IsRead = isRead;
        }
    }
}
