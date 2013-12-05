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
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Controls
{
    /// <summary>
    /// Interaction logic for EditIndicators.xaml
    /// </summary>
    public partial class EditIndicators : UserControl, IDisposable
    {
        public EditIndicators()
        {
            InitializeComponent();
            this.DataContext = new EditIndicatorsVm();
            //dgTaxIndicators.ItemsSource = new List<object>() { 1 };
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataContext = this.DataContext as EditIndicatorsVm;
            ComboBox cmb = sender as ComboBox;
            var cmbDc = cmb.DataContext as TaxIndicatorViewModel;
            cmbDc.Type = (TaxIndicatorType)cmb.SelectedItem;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as EditIndicatorsVm;
            if (dataContext.SelectedItem != null)
            {
                int maxNrCrt = dataContext.GetMaxNrCrt();
                //verify the item with the inner id 30 instead of the value (in case multiple rows are added)
                //what to do when the row was deleted?                
            }
        }


        #region textbox arrow navigation
        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var txtBox = (sender as TextBox);
            //if (txtBox.Tag != null && txtBox.Tag is bool && (bool)txtBox.Tag == true)
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
            catch { }
        }

        private static void FocusTextBox(TextBox nextTextbox)
        {
            nextTextbox.Focus();
            nextTextbox.SelectAll();
        }

        int counter = 1;
        List<TextBox> alltextboxes = new List<TextBox>();

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
        #endregion
    }
}
