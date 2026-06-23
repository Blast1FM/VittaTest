using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VittaTest.Models;
using VittaTest.Services;
using VittaTest.Views;

namespace VittaTest.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IOrderPaymentService _orderPaymentService;
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
        }

        [RelayCommand]
        private void OpenSelectOrderWindow()
        {
            var window = App.ServiceProvider.GetRequiredService<SelectOrderWindow>(); 
            if (window.ShowDialog() == true)
            {
                SelectedOrder = window?.SelectedOrder;
            }
        }

        [RelayCommand]
        private void OpenSelectInflowWindow()
        {
            var window = App.ServiceProvider.GetRequiredService<SelectCashInflowWindow>();
            if (window.ShowDialog() == true)
            {
                SelectedInflow = window?.SelectedInflow;
            }
        }

        [RelayCommand]
        private void OpenPaymentsWindow()
        {
            var window = App.ServiceProvider.GetRequiredService<PaymentsWindow>();
            window.ShowDialog();
        }

        [RelayCommand]
        private void ClearOrder() => SelectedOrder = null;

        [RelayCommand]
        private void ClearInflow() => SelectedInflow = null;

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
                PaymentAmount = 0;

                MessageBox.Show("Платёж успешно выполнен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка в бд при выполнении платежа:\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
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

