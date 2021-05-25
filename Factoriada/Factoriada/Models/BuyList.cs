using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class BuyList
    {
        public Guid BuyListId { get; set; }
        public ToBuy ToBuy { get; set; }
        public bool Hidden { get; set; }

        public User Owner { get; set; }

        public ApartmentDetail ApartmentDetail { get; set; }
    }
}