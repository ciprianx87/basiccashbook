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
using System.Windows.Threading;
using CashBook.Common;
using CashBook.Controls.Printing;
using System.Printing;

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl, IDisposable
    {
        public Report()
        {
            InitializeComponent();
            this.DataContext = new ReportsViewModel();
            var printedPage = (grdReport.Children[0] as PrintedPage);
            printedPage.Loaded += new RoutedEventHandler(printedPage_Loaded);
        }

        void printedPage_Loaded(object sender, RoutedEventArgs e)
        {
            var lstItems = (sender as PrintedPage).GetLstItems();
            var height = lstItems.ActualHeight;
            int maxRowsPerPage = (int)height / rowHeight;
            (this.DataContext as ReportsViewModel).SetMaxEntriesPerPage(maxRowsPerPage);
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog dialog = new PrintDialog();
                dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

                if (dialog.ShowDialog() != true) return;

                grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                //grdReport.Arrange(new Rect(new Point(0, 0), grdReport.DesiredSize));

                dialog.PrintVisual(grdReport, "Report");
                //827*1169
                //Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
        private const int rowHeight = 20;
       
    }
}
