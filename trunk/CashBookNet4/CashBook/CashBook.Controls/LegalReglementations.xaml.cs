using System;
using System.Collections.Generic;
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
using CashBook.ViewModels;

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for LegalReglementations.xaml
    /// </summary>
    public partial class LegalReglementations : UserControl, IDisposable
    {
        public LegalReglementations()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(LegalReglementations_Loaded);
        }

        void LegalReglementations_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new LegalReglementationsViewModel();
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
            
        }
    }
}
