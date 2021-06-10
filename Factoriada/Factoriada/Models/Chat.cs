using System;

namespace Factoriada.Models
{
    public class Chat
    {
        public Guid ChatId { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public Guid ApartmentId { get; set; }
        public string ChatMessage => ($"{DateTime:g}-{User.LastName} {User.FirstName}:{Message}");
    }
}