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
using System.Windows.Shapes;
using TaxCalculator.ViewModel.ViewModels;
using TaxCalculator.ViewModel.Base;

namespace TaxCalculator.Controls.Popups
{
    /// <summary>
    /// Interaction logic for CreateEditCompany.xaml
    /// </summary>
    public partial class CreateEditCompany : Window, IDisposable
    {
        public CreateEditCompany()
        {
            InitializeComponent();
            this.DataContext = new CreateEditCompanyVm();
            txtName.Focus();
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();            
        }
    }
}
