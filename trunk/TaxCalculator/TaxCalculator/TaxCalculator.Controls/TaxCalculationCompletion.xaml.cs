using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaxCalculator.ViewModel.ViewModels;
using TaxCalculator.ViewModel.Base;

namespace TaxCalculator.Controls
{
    /// <summary>
    /// Interaction logic for TaxCalculationCompletion.xaml
    /// </summary>
    public partial class TaxCalculationCompletion : UserControl, IDisposable
    {
        public TaxCalculationCompletion()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(TaxCalculationCompletion_DataContextChanged);
            this.DataContext = new TaxCalculationCompletionVm();
            (this.DataContext as TaxCalculationCompletionVm).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TaxCalculationCompletion_PropertyChanged);
            //dgTaxIndicators.ItemsSource = new List<object>() { 1 };
        }

        void TaxCalculationCompletion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Rectifying")
            {
                var initialValueColumn = dgTaxIndicators.Columns.FirstOrDefault(p => p.Header != null && p.Header.ToString() == "Valoare Initiala");
                if (initialValueColumn != null)
                {
                    initialValueColumn.Visibility = (this.DataContext as TaxCalculationCompletionVm).Rectifying;
                }
            }
        }

        void TaxCalculationCompletion_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
          

        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void AddBefore_Click(object sender, RoutedEventArgs e)
        {
            //var selectedRow = dgTaxIndicators.SelectedItem;
            //this.DataContext = new TaxCalculationVm();
        }

        private void AddAfter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //return;
            TextBox txtBox = sender as TextBox;
            txtBox.SelectAll();
            e.Handled = true;
            txtBox.Focus();

        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        private void TextBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key==Key.Down)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                MoveFocus(request);
            }
            if (e.Key == Key.U)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                MoveFocus(request);
            }
        }

        private void dgTaxIndicators_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

    }
}