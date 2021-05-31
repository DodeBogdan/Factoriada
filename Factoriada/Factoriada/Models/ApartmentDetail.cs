using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class ApartmentDetail
    {
        public Guid ApartmentDetailId { get; set; }
        public float UnspentMoney { get; set; }
        public string ApartmentName { get; set; }
        public string Token { get; set; }
        public User Owner { get; set; }
        public Address ApartmentAddress { get; set; }
    }
}