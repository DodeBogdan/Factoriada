using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class Address
    {
        public Guid AddressId { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Building { get; set; }
        public string Staircase { get; set; }
        public string Floor { get; set; }
        public string Apartment { get; set; }

    }
}