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
    /// Interaction logic for TaxCalculationList.xaml
    /// </summary>
    public partial class TaxCalculationList : UserControl, IDisposable
    {
        public TaxCalculationList(bool isRectifying)
        {
            InitializeComponent();
            this.DataContext = new TaxCalculationListVm(isRectifying);
        }

        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxCalculationListVm).Delete(but.CommandParameter);
        }
        private void EditCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxCalculationListVm).Edit(but.CommandParameter);
        }

        private void VizualizareCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxCalculationListVm).View(but.CommandParameter);
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
