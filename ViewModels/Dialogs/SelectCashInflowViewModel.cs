using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using VittaTest.Models;
using VittaTest.Services;

namespace VittaTest.ViewModels.Dialogs
{
    public partial class SelectCashInflowViewModel : ObservableObject
    {
        private readonly IOrderPaymentService _service;

        public ObservableCollection<CashInflow> CashInflows { get; } = new();
        public ICollectionView CashInflowsView { get; }

        [ObservableProperty]
        public CashInflow? selectedInflow;

        [ObservableProperty]
        public string searchText = string.Empty;

        [ObservableProperty]
        private string searchColumnPath = "InflowNumber";

        public SelectCashInflowViewModel(IOrderPaymentService service)
        {
            _service = service;
            CashInflowsView = CollectionViewSource.GetDefaultView(CashInflows);

            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var inflows = await _service.GetAllCashInflowsAsync();
                CashInflows.Clear();
                foreach (var inflow in inflows)
                    CashInflows.Add(inflow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки поступлений: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Select() { }

        [RelayCommand]
        private void Cancel() { }
    }
}
