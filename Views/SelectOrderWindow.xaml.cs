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

namespace VittaTest.Views
{
    /// <summary>
    /// Interaction logic for SelectOrderWindow.xaml
    /// </summary>
    public partial class SelectOrderWindow : Window
    {
        public SelectOrderWindow()
        {
            InitializeComponent();
        }
        public Order? SelectedOrder => (DataContext as SelectOrderViewModel)?.SelectedOrder;
    }
}
