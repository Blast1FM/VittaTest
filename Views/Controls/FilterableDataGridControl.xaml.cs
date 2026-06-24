using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VittaTest.Controls
{
    [ContentProperty("Columns")]
    public partial class FilterableDataGridControl : UserControl
    {
        public static readonly DependencyProperty ItemsViewProperty =
            DependencyProperty.Register(nameof(ItemsView), typeof(ICollectionView),
                typeof(FilterableDataGridControl));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object),
                typeof(FilterableDataGridControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string),
                typeof(FilterableDataGridControl),
                new PropertyMetadata(string.Empty, OnSearchCriteriaChanged));

        public static readonly DependencyProperty SearchColumnPathProperty =
            DependencyProperty.Register(nameof(SearchColumnPath), typeof(string),
                typeof(FilterableDataGridControl),
                new PropertyMetadata(string.Empty, OnSearchCriteriaChanged));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(ObservableCollection<DataGridColumn>),
                typeof(FilterableDataGridControl), new PropertyMetadata(null, OnColumnsPropertyChanged));

        public FilterableDataGridControl()
        {
            InitializeComponent();
            Columns = new ObservableCollection<DataGridColumn>();
        }

        public ICollectionView ItemsView
        {
            get => (ICollectionView)GetValue(ItemsViewProperty);
            set => SetValue(ItemsViewProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public string SearchColumnPath
        {
            get => (string)GetValue(SearchColumnPathProperty);
            set => SetValue(SearchColumnPathProperty, value);
        }

        public ObservableCollection<DataGridColumn> Columns
        {
            get => (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        private static void OnSearchCriteriaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FilterableDataGridControl)d;
            control.PerformSearchAndSelect();
        }

        private void PerformSearchAndSelect()
        {
            if (ItemsView == null || ItemsView.IsEmpty)
            {
                SelectedItem = null;
                return;
            }

            string search = SearchText?.Trim();
            string path = SearchColumnPath?.Trim();

            if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(path))
            {
                SelectedItem = null;
                return;
            }

            foreach (object item in ItemsView)
            {
                if (item == null) continue;

                PropertyInfo prop = item.GetType().GetProperty(path,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null) continue;

                object value = prop.GetValue(item);
                string valueString = value?.ToString() ?? string.Empty;

                if (string.Equals(valueString, search, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedItem = item;
                    InnerDataGrid.ScrollIntoView(item);
                    return;
                }
            }

            SelectedItem = null;
        }

        private static void OnColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FilterableDataGridControl)d;
            if (e.OldValue is ObservableCollection<DataGridColumn> oldCol)
                oldCol.CollectionChanged -= control.OnColumnsCollectionChanged;

            if (e.NewValue is ObservableCollection<DataGridColumn> newCol)
            {
                newCol.CollectionChanged += control.OnColumnsCollectionChanged;
                control.SyncColumns(newCol);
            }
        }

        private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<DataGridColumn> columns)
                SyncColumns(columns);
        }

        private void SyncColumns(ObservableCollection<DataGridColumn> source)
        {
            InnerDataGrid.Columns.Clear();
            foreach (var col in source)
                InnerDataGrid.Columns.Add(col);
        }
    }
}