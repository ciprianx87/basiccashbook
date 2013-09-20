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
    /// Interaction logic for InformationPopup.xaml
    /// </summary>
    public partial class InformationPopup : Window, IDisposable
    {
        public InformationPopup(bool modal)
        {
            InitializeComponent();
            this.DataContext = new InformationDialogVm(modal);
        }
        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
