using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VittaTest.Models;
using VittaTest.ViewModels.Dialogs;
using VittaTest.ViewModels.Messages;

namespace VittaTest.Views
{
    /// <summary>
    /// Interaction logic for SelectOrderWindow.xaml
    /// </summary>
    public partial class SelectOrderWindow : Window
    {
        private bool _isClosing;   
        public SelectOrderWindow()
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
        public Order? SelectedOrder => (DataContext as SelectOrderViewModel)?.SelectedOrder;
    }
}
