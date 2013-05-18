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
using CashBook.ViewModels;
using System.Windows.Controls.Primitives;
using System.Threading;
using CashBook.Controls.CustomDataGrid;
using CashBook.Common;
using CashBook.Controls;
using CashBook.Controls.Printing;


namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for PrintControl.xaml
    /// </summary>
    public partial class PrintControl : UserControl, IDisposable
    {
        public PrintControl()
        {
            InitializeComponent();
            this.DataContext = new PrintControlVM();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintAllPages();
                //Print();
                //PrintDialog dialog = new PrintDialog();

                //if (dialog.ShowDialog() != true) return;

                //grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                ////grdReport.Arrange(new Rect(new Point(50, 50), grdReport.DesiredSize));

                //dialog.PrintVisual(grdReport, "Report");
                //827*1169
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
        private void Print()
        {
            PrintedPage pp = new PrintedPage();
            PrintDialog dialog = new PrintDialog();

            if (dialog.ShowDialog() != true) return;


            for (int i = 0; i < 3; i++)
            {
                var printedPage = new PrintedPage();
                printedPage.DataContext = new TestClass()
                {
                    Test = "test " + i
                };

                grdReport.Children.Clear();
                grdReport.Children.Add(printedPage);

                grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                //grdReport.Arrange(new Rect(new Point(50, 50), grdReport.DesiredSize));
                //grdReport.Visibility = System.Windows.Visibility.Hidden;

                dialog.PrintVisual(grdReport, "Raport");
            }

        }

        public void PrintAllPages()
        {
            grdReport.Children.Clear();
            Print();
            //set the datacontext for each page and send it to the printer
           
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }

    public class TestClass
    {
        public string Test { get; set; }
    }
}
