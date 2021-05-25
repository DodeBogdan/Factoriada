using System;
using System.Collections.Generic;
using System.Text;

namespace Factoriada.Models
{
    class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address AddressPlace { get; set; }
    }
}
