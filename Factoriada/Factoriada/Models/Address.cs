using System;
using System.Collections.Generic;
using System.Text;

namespace Factoriada.Models
{
    class Address
    {
        public Guid AddressId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
    }
}
