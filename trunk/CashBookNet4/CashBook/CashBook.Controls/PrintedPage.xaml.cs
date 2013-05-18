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
using CashBook.ViewModels;
using System.ComponentModel;

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for PrintedPage.xaml
    /// </summary>
    public partial class PrintedPage : UserControl
    {
        public PrintedPage()
        {
            InitializeComponent();
        }

        private const int rowHeight = 20;
        private void lstItems_Loaded(object sender, RoutedEventArgs e)
        {
            //return;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var height = lstItems.ActualHeight;
                int maxRowsPerPage = (int)height / rowHeight;
                (this.DataContext as ReportsViewModel).SetMaxEntriesPerPage(maxRowsPerPage);
            }
        }
    }
}
