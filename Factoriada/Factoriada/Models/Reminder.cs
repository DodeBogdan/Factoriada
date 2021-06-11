using System;

namespace Factoriada.Models
{
    public class Reminder
    {
        public Guid ReminderId { get; set; }
        public Guid ApartmentDetail { get; set; }
        public string Message { get; set; }

    }
}