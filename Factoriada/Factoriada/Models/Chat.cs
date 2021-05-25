using System;

namespace Factoriada.Models
{
    public class Chat
    {
        public Guid ChatId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public ApartmentDetail ApartmentDetail { get; set; }
    }
}