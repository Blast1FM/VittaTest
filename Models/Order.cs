using CommunityToolkit.Mvvm.ComponentModel;

namespace VittaTest.Models
{
    public partial class Order : ObservableObject
    {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
