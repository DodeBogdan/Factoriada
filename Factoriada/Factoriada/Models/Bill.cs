using System;
using System.Collections.Generic;

namespace Factoriada.Models
{
    public class Bill
    {
        public Guid BillId { get; set; }
        public float BillPrice { get; set; }
        public BillType Type { get; set; }
        public bool Paid { get; set; }
        public DateTime DateOfIssue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }

        public Guid ApartmentDetail { get; set; }

        public static List<BillType> GetBillTypes()
        {
            var list = new List<BillType>()
            {
                BillType.Curent,
                BillType.Gaz,
                BillType.Internet,
                BillType.Intretinere
            };

            return list;
        }
    }
}