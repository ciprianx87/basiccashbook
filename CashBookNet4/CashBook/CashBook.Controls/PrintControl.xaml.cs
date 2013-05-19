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
using CashBook.Common.Mediator;
using System.Windows.Threading;


namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for PrintControl.xaml
    /// </summary>
    public partial class PrintControl : UserControl, IDisposable
    {
        private const int rowHeight = 20;
        public PrintControl()
        {
            InitializeComponent();
            this.DataContext = new PrintControlVM();
            Mediator.Instance.Register(MediatorActionType.StartPrinting, StartPrinting);
            this.Loaded += new RoutedEventHandler(PrintControl_Loaded);
        }

        void PrintControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(PrintControl_Loaded);
            PrintedPage pp = new PrintedPage();
            pp.Loaded += new RoutedEventHandler(pp_Loaded);
            grdReport.Children.Add(pp);
        }

        void pp_Loaded(object sender, RoutedEventArgs e)
        {
            var lstItems=(sender as PrintedPage).GetLstItems();
            var height = lstItems.ActualHeight;
            int maxRowsPerPage = (int)height / rowHeight;
            (this.DataContext as PrintControlVM).SetMaxEntriesPerPage(maxRowsPerPage);
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
            //Print();
            //set the datacontext for each page and send it to the printer

        }
        public void StartPrinting(object param)
        {
            if (param != null)
            {
                grdReport.Children.Clear();
                List<ReportPageVM> pagesToPrint = param as List<ReportPageVM>;
                PrintVmPages(pagesToPrint);
            }

        }

        private void PrintVmPages(List<ReportPageVM> pagesToPrint)
        {
            if (pagesToPrint == null || pagesToPrint.Count == 0)
            {
                return;
            }
            PrintedPage pp = new PrintedPage();
            PrintDialog dialog = new PrintDialog();

            if (dialog.ShowDialog() != true) return;

            foreach (var page in pagesToPrint)
            {
                var printedPage = new PrintedPage();
                printedPage.DataContext = page;
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                grdReport.Children.Clear();
                grdReport.Children.Add(printedPage);

                grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                //grdReport.Arrange(new Rect(new Point(50, 50), grdReport.DesiredSize));
                //grdReport.Visibility = System.Windows.Visibility.Hidden;

                dialog.PrintVisual(grdReport, "Raport");
            }

        }

        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.StartPrinting, StartPrinting);
            (this.DataContext as BaseViewModel).Dispose();
        }
    }

    public class TestClass
    {
        public string Test { get; set; }
    }
}
