using System;
using System.Collections.Generic;
using System.Text;

namespace Factoriada.Models
{
    public class BudgetHistory
    {
        public Guid BudgetHistoryId { get; set;}
        public User User { get; set; }
        public float Amount { get; set; }
    }
}
