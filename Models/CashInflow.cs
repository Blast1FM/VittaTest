using CommunityToolkit.Mvvm.ComponentModel;

namespace VittaTest.Models
{
    public partial class CashInflow : ObservableObject
    {
        public int InflowNumber { get; set; }
        public DateTime InflowDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Remaining { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
