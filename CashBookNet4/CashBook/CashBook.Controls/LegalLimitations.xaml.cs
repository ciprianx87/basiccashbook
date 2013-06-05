using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using CashBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for LegalLimitations.xaml
    /// </summary>
    public partial class LegalLimitations : UserControl, IDisposable
    {
        
        public LegalLimitations()
        {
            InitializeComponent();
            this.Loaded += CompanyDataWindow_Loaded;
            this.DataContext = new LegalLimitationsVM();          

        }

        void CompanyDataWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CompanyDataWindow_Loaded;
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
