using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class Bill
    {
        public Guid BillId { get; set; }
        public string BillName { get; set; }
        public float BillPrice { get; set; }
        public string Type { get; set; }
        public bool Paid { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime DueDate { get; set; }

        public ApartmentDetail ApartmentDetail { get; set; }
    }
}