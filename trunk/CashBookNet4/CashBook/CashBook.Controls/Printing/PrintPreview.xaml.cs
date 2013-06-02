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
using CashBook.Common;
using System.Printing;

namespace CashBook.Controls.Printing
{
    /// <summary>
    /// Interaction logic for PreviewReport.xaml
    /// </summary>
    public partial class PrintPreview : UserControl, IDisposable
    {
        public PrintPreview()
        {
            InitializeComponent();
            this.DataContext = new PrintPreviewVM();
            //this.Loaded += new RoutedEventHandler(PrintControl_Loaded);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

                dialog.PrintVisual(grdReport, "Raport");
                //827*1169
                //Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        public void Dispose()
        {        
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
