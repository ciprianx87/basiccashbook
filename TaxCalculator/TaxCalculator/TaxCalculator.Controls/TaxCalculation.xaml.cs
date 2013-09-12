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

namespace TaxCalculator.Controls
{
    /// <summary>
    /// Interaction logic for TaxCalculation.xaml
    /// </summary>
    public partial class TaxCalculation : UserControl
    {
        public TaxCalculation()
        {
            InitializeComponent();
            this.DataContext = new TaxCalculationVm();
        }
    }
}
