using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using VittaTest.Models;
using VittaTest.Services;
using VittaTest.ViewModels.Messages;

namespace VittaTest.ViewModels.Dialogs
{
    public partial class SelectOrderViewModel : ObservableObject
    {
        private readonly IOrderPaymentService _service;

        public ObservableCollection<Order> Orders { get; } = new();
        public ICollectionView OrdersView { get; }

        [ObservableProperty] 
        private string searchText = string.Empty;
        [ObservableProperty] 
        private Order? selectedOrder;
        [ObservableProperty] 
        private string searchColumnPath = "OrderNumber";

        public SelectOrderViewModel(IOrderPaymentService service)
        {
            _service = service;

            OrdersView = CollectionViewSource.GetDefaultView(Orders);
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var orders = await _service.GetAllOrdersAsync();
                Orders.Clear();
                foreach (var order in orders)
                    Orders.Add(order);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Select()
        {
            WeakReferenceMessenger.Default.Send(new CloseWindowMessage(true));
        }

        [RelayCommand]
        private void Cancel()
        {
            WeakReferenceMessenger.Default.Send(new CloseWindowMessage(false));
        }
    }
}