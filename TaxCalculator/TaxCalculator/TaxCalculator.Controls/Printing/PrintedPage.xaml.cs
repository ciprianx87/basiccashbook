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

namespace TaxCalculator.Controls.Printing
{
    /// <summary>
    /// Interaction logic for PrintedPage.xaml
    /// </summary>
    public partial class PrintedPage : UserControl
    {
        public PrintedPage()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(PrintedPage_DataContextChanged);
        }

        void PrintedPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
