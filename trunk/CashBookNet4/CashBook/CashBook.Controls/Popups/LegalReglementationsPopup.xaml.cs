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
using CashBook.ViewModels;

namespace CashBook.Controls.Popups
{
    /// <summary>
    /// Interaction logic for LegalReglementationsPopup.xaml
    /// </summary>
    public partial class LegalReglementationsPopup : Window, IDisposable
    {
        public LegalReglementationsPopup()
        {
            InitializeComponent();
            this.Loaded += LegalReglementationsPopup_Loaded;
        }

        void LegalReglementationsPopup_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new LegalReglementationsPopupViewModel();
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();

        }
    }
}
