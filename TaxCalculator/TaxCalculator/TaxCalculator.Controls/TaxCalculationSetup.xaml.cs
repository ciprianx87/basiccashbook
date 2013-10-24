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
    /// Interaction logic for TaxCalculationSetup.xaml
    /// </summary>
    public partial class TaxCalculationSetup : UserControl, IDisposable
    {
        public TaxCalculationSetup()
        {
            InitializeComponent();
            this.DataContext = new TaxCalculationSetupVm();
        }
        
        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
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
    }
}
