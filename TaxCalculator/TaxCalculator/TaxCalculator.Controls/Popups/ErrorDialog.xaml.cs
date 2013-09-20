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
using TaxCalculator.ViewModel.Base;
using TaxCalculator.ViewModel.ViewModels.Popups;

namespace TaxCalculator.Controls.Popups
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window, IDisposable
    {
        public ErrorDialog()
        {
            InitializeComponent();
            this.DataContext = new ErrorDialogVm();
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
