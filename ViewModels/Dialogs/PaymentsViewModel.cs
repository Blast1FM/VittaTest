using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using VittaTest.Models;
using VittaTest.Services;

namespace VittaTest.ViewModels.Dialogs
{
    public partial class PaymentsViewModel : ObservableObject
    {
        private readonly IOrderPaymentService _service;
        public ObservableCollection<Payment> Payments { get; set; } = new();
        public PaymentsViewModel(IOrderPaymentService service)
        {
            _service = service;
            LoadPaymentsAsync().ConfigureAwait(false);
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                var payments = await _service.GetAllPaymentsAsync();
                Payments.Clear();
                foreach (var payment in payments)
                    Payments.Add(payment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки платежей: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}