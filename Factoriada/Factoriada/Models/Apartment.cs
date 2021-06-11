using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class Apartment
    {
        public Guid ApartmentId { get; set; }
        public Guid ApartmentDetail { get; set; }
        public User User { get; set; }
    }
}