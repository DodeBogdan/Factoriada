using System;

namespace Factoriada.Models
{
    public class Announce
    {
        public Guid AnnounceId { get; set; }
        public User User { get; set; }
        public string AnnounceMessage { get; set; }
        public Guid ApartmentDetails { get; set; }
        public DateTime InsertedDateTime { get; set; }
    }
}