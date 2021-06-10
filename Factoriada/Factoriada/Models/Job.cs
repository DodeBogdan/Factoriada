using System;
using System.Collections.Generic;
using System.Text;

namespace Factoriada.Models
{
    public class Job
    {
        public Guid JobId { get; set; }
        public string JobType { get; set; }
        public string User { get; set; }
        public ApartmentDetail ApartmentDetail { get; set; }
    }
}
