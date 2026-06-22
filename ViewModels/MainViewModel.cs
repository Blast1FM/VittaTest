using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using VittaTest.Models;
using VittaTest.Services;

namespace VittaTest.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IOrderPaymentService _orderPaymentService;
        public ObservableCollection<Order> Orders { get; } = new();
        public ObservableCollection<CashInflow> CashInflows { get; } = new();
        public ObservableCollection<Payment> Payments { get; } = new();
        [ObservableProperty] 
        public Order? selectedOrder;
        [ObservableProperty]
        public CashInflow? selectedInflow;
        [ObservableProperty]
        public decimal paymentAmount;
        [ObservableProperty]
        public bool canMakePayment;

        public MainViewModel(IOrderPaymentService orderPaymentService)
        {
            _orderPaymentService = orderPaymentService;
            LoadDataAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            try
            {
                Orders.Clear();
                CashInflows.Clear();
                Payments.Clear();

                var orders = _orderPaymentService.GetAllOrdersAsync();
                var inflows = _orderPaymentService.GetAllCashInflowsAsync();
                var payments = _orderPaymentService.GetAllPaymentsAsync();

                await Task.WhenAll(orders, inflows, payments);

                foreach (var o in await orders) Orders.Add(o);
                foreach (var ci in await inflows) CashInflows.Add(ci);
                foreach (var p in await payments) Payments.Add(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task MakePaymentAsync()
        {
            if (SelectedOrder == null || SelectedInflow == null || PaymentAmount <= 0)
            {
                MessageBox.Show("Выберите заказ, поступление и укажите сумму платежа.", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var payment = new Payment
            {
                OrderNumber = SelectedOrder.OrderNumber,
                InflowNumber = SelectedInflow.InflowNumber,
                PaymentAmount = PaymentAmount,
                ExpectedOrderVersion = SelectedOrder.RowVersion,
                ExpectedInflowVersion = SelectedInflow.RowVersion
            };
            
            try
            {
                await _orderPaymentService.MakePaymentAsync(payment);
                await LoadDataAsync();
                PaymentAmount = 0;

                MessageBox.Show("Платёж успешно выполнен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                await LoadDataAsync();
                MessageBox.Show($"Ошибка в бд при выполнении платежа:\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (Exception ex)
            {
                await LoadDataAsync();
                MessageBox.Show($"Ошибка при выполнении платежа:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateCanMakePayment()
        {
            CanMakePayment = PaymentAmount > 0 &&
                             SelectedOrder != null &&
                             SelectedInflow != null;
        }
        // Конечно по хорошему это расчитываемое свойство можно сделать покрасивее
        partial void OnSelectedInflowChanged(CashInflow? value)
        {
            UpdateCanMakePayment();
        }
        partial void OnSelectedOrderChanged(Order? value)
        {
            UpdateCanMakePayment();
        }
        partial void OnPaymentAmountChanged(decimal value)
        {
            UpdateCanMakePayment();
        }
    }
}

