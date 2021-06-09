using System;

namespace Factoriada.Models
{
    public class BillPaidPersons
    {
        public Guid BillPaidId { get; set; }
        public Guid BillPaidBillId { get; set; }
        public User BillUserPaid { get; set; }
        public float MoneyPaid { get; set; }
    }
}