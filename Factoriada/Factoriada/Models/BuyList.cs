using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class BuyList
    {
        public Guid BuyListId { get; set; }
        public string Product { get; set; }
        public int Count { get; set; }
        public bool Hidden { get; set; }
        public User Owner { get; set; }
        public Guid ApartmentDetail { get; set; }
    }
}