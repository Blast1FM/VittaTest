using System;
using System.Collections.Generic;

namespace VittaTest.Models;

public partial class Order
{
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public byte[] RowVersion { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
