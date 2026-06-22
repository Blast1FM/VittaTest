using Microsoft.EntityFrameworkCore;
using VittaTest.Data;
using VittaTest.Models;

namespace VittaTest.Services
{
    internal class OrderPaymentService : IOrderPaymentService
    {
        private readonly IDbContextFactory<OrderAccountingDbContext> _contextFactory;
        public OrderPaymentService(IDbContextFactory<OrderAccountingDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Orders.AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<CashInflow>> GetAllCashInflowsAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.CashInflows.AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Payments.AsNoTracking().ToListAsync();
            }
        }

        public async Task MakePaymentAsync(Payment payment)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Payments.Add(payment);
                await context.SaveChangesAsync();
            }
        }
    }
}
