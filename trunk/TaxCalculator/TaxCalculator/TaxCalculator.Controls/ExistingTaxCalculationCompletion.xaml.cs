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
    /// Interaction logic for ExistingTaxCalculationCompletion.xaml
    /// </summary>
    public partial class ExistingTaxCalculationCompletion : UserControl, IDisposable
    {
        public ExistingTaxCalculationCompletion()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(TaxCalculationCompletion_DataContextChanged);
            this.DataContext = new ExistingTaxCalculationCompletionVm();
            (this.DataContext as ExistingTaxCalculationCompletionVm).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TaxCalculationCompletion_PropertyChanged);
            //dgTaxIndicators.ItemsSource = new List<object>() { 1 };
        }

        void TaxCalculationCompletion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Rectifying")
            {
                var initialValueColumn = dgTaxIndicators.Columns.FirstOrDefault(p => p.Header != null && p.Header.ToString() == "Valoare Initiala");
                if (initialValueColumn != null)
                {
                    initialValueColumn.Visibility = (this.DataContext as ExistingTaxCalculationCompletionVm).Rectifying;
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
            if (e.Key == Key.Return || e.Key == Key.Down)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                MoveFocus(request);
            }
            if (e.Key == Key.Up)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                MoveFocus(request);
            }
        }

        private void dgTaxIndicators_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int currentCounter = Convert.ToInt32((sender as TextBox).Tag);
            if (e.Key == Key.Return || e.Key == Key.Down)
            {
                SelectTextBoxById(currentCounter + 1);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                SelectTextBoxById(currentCounter - 1);
                e.Handled = true;
            }
        }
        private void SelectTextBoxById(int id)
        {
            try
            {
                if (id < 0 || id > alltextboxes.Count)
                {
                    return;
                }
                var nextTextbox = alltextboxes.FirstOrDefault(p => p.Tag != null && p.Tag.ToString() == id.ToString());
                if (nextTextbox != null)
                {
                    FocusTextBox(nextTextbox);
                }
            }
            catch{}
        }

        private static void FocusTextBox(TextBox nextTextbox)
        {
            nextTextbox.Focus();
            nextTextbox.SelectAll();
        }

        int counter = 1;
        List<TextBox> alltextboxes = new List<TextBox>();
        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var txtBox = (sender as TextBox);
            if (txtBox.Tag != null && txtBox.Tag is Visibility && (Visibility)txtBox.Tag == Visibility.Visible)
            {
                txtBox.Tag = counter.ToString();
                //txtBox.Text = counter.ToString();
                if (counter == 1)
                {
                    //select the first textbox
                    FocusTextBox(txtBox);
                }
                counter++;
                alltextboxes.Add(txtBox);
                
            }
        }

    }
}