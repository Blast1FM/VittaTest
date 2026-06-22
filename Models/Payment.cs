using System;
using System.Collections.Generic;
using System.Text;

namespace VittaTest.Models
{
    public class Payment
    {
        public Guid PaymentID { get; set; }
        public int OrderNumber { get; set; }
        public int InflowNumber { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // Optimistic concurrency
        public byte[]? ExpectedOrderVersion { get; set; }
        public byte[]? ExpectedInflowVersion { get; set; }
    }
}
