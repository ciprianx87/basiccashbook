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
                //int rd30Position=da
                //if (maxNrCrt > 30)
                //{
                //    contextMenu.IsOpen = false;
                //    e.Handled = true;
                //}
            }
        }

    }
}
