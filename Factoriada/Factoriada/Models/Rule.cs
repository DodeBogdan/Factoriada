using System;

namespace Factoriada.Models
{
    public class Rule
    {
        public Guid RuleId { get; set; }
        public string RuleMessage { get; set; }
        public Guid ApartmentDetail { get; set; }
        public DateTime InsertedDateTime { get; set; }
    }
}