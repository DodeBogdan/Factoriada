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
        public Guid ApartmentDetail { get; set; }
        public DateTime InsertedDateTime { get; set; }

        public string ShowBudget
        {
            get
            {
                if (Amount >= 0)
                    return ($"{User.LastName} {User.FirstName} a adaugat {Amount}");
                else
                    return ($"{User.LastName} {User.FirstName} a extras {Amount}");
            }
        }
    }
}
