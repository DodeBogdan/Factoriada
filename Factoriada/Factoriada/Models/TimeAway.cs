using System;

namespace Factoriada.Models
{
    public class TimeAway
    {
        public Guid TimeAwayId { get; set; }
        public User User { get; set; }
        public Guid ApartmentDetail { get; set; }
        public DateTime LeaveFrom { get; set; }
        public DateTime LeaveTo { get; set; }
        public int DaysLeft { get; set; }
    }
}