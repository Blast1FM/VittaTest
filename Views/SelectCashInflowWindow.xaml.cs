using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VittaTest.Models;
using VittaTest.ViewModels.Dialogs;

namespace VittaTest.Views
{
    /// <summary>
    /// Interaction logic for SelectCashInflowWindow.xaml
    /// </summary>
    public partial class SelectCashInflowWindow : Window
    {
        public SelectCashInflowWindow()
        {
            InitializeComponent();
        }
        public CashInflow? SelectedInflow => (DataContext as SelectCashInflowViewModel)?.SelectedInflow;
    }
}
