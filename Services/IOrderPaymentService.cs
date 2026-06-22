using VittaTest.Models;

namespace VittaTest.Services
{
    public interface IOrderPaymentService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<CashInflow>> GetAllCashInflowsAsync();
        Task<List<Payment>> GetAllPaymentsAsync();
        Task MakePaymentAsync(Payment payment);
    }
}
