using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VittaTest.Models;
using VittaTest.ViewModels.Dialogs;
using VittaTest.ViewModels.Messages;

namespace VittaTest.Views
{
    /// <summary>
    /// Interaction logic for SelectCashInflowWindow.xaml
    /// </summary>
    public partial class SelectCashInflowWindow : Window
    {
        private bool _isClosing;
        public SelectCashInflowWindow()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<CloseWindowMessage>(this, (r, m) =>
            {
                // Вонючий хак, т.к. если выбрана запись, похоже что при отмене ивент проходит несколько раз, а времени разбираться с этим уже нет
                if (_isClosing) return;
                _isClosing = true;

                DialogResult = m.DialogResult;
                Close();
            });
        }
        public CashInflow? SelectedInflow => (DataContext as SelectCashInflowViewModel)?.SelectedInflow;
    }
}
