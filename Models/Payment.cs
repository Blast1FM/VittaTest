using System;
using System.Collections.Generic;

namespace VittaTest.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }
    public int OrderNumber { get; set; }
    public int InflowNumber { get; set; }
    public decimal PaymentAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public byte[]? ExpectedOrderVersion { get; set; }
    public byte[]? ExpectedInflowVersion { get; set; }
    public virtual CashInflow InflowNumberNavigation { get; set; } = null!;
    public virtual Order OrderNumberNavigation { get; set; } = null!;
}
