using System;
using System.Collections.Generic;

namespace VittaTest.Models;

public partial class CashInflow
{
    public int InflowNumber { get; set; }
    public DateTime InflowDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Remaining { get; set; }
    public byte[] RowVersion { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
