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
    /// Interaction logic for CompanyList.xaml
    /// </summary>
    public partial class CompanyList : UserControl
    {
        public CompanyList()
        {
            InitializeComponent();
            this.DataContext = new CompanyListVm();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as CompanyListVm).Select(but.CommandParameter);
        }
        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as CompanyListVm).Delete(but.CommandParameter);
        }
        private void EditCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as CompanyListVm).Edit(but.CommandParameter);
        }
    }
}
