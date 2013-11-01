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
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common;
using TaxCalculator.ViewModel.ViewModels.Model;
using System.Printing;
using TaxCalculator.Common.Mediator;
using Microsoft.Win32;

namespace TaxCalculator.Controls.Printing
{
    /// <summary>
    /// Interaction logic for PrintControl.xaml
    /// </summary>
    public partial class PrintControl : UserControl, IDisposable
    {
        public PrintControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PrintControl_Loaded);
            this.DataContext = new PrintPreviewVm();
            Mediator.Instance.Register(MediatorActionType.StartPrinting, StartPrinting);
        }

        void PrintControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void StartPrinting(object param)
        {
            try
            {
                if (param != null)
                {
                    List<PrintPage> pagesToPrint = param as List<PrintPage>;
                    PrintVmPagesUpdated(pagesToPrint);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
        

        private void PrintVmPagesUpdated(List<PrintPage> pagesToPrint)
        {
            try
            {
                if (pagesToPrint == null || pagesToPrint.Count == 0)
                {
                    Logger.Instance.Log.Debug("pagesToPrint == null || pagesToPrint.Count == 0");
                    return;
                }
                Logger.Instance.Log.Debug("trying to print");
                // PrintedPage pp = new PrintedPage();
                PrintDialog dialog = new PrintDialog();
                dialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
                dialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

                if (dialog.ShowDialog() != true) return;

                FixedDocument fixedDocument = new FixedDocument();

                foreach (var page in pagesToPrint)
                {
                    Logger.Instance.Log.Debug("trying to print page");
                    var printedPage = new PrintedPage();
                    printedPage.DataContext = page;
                    //Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                    grdReport = new Grid();
                    grdReport.Children.Clear();
                    grdReport.Children.Add(printedPage);
                    //Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                    Logger.Instance.Log.Debug("before Measure");
                    Logger.Instance.Log.Debug(string.Format("dialog.PrintableAreaWidth {0}, dialog.PrintableAreaHeight {1}", dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                    grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                    Logger.Instance.Log.Debug("UpdateLayout");
                    grdReport.UpdateLayout();
                    //grdReport.Arrange(new Rect(new Point(50, 50), grdReport.DesiredSize));
                    //grdReport.Visibility = System.Windows.Visibility.Hidden;

                    // dialog.PrintVisual(grdReport, "Raport");
                    Logger.Instance.Log.Debug("after UpdateLayout");

                    PageContent pageContent = new PageContent();
                    FixedPage pg = new FixedPage();
                    //page.Width = capabilities.PageImageableArea.ExtentWidth;
                    //page.Height = capabilities.PageImageableArea.ExtentHeight;

                    //container holds our visual...  
                    //VisualContainer myContainer = new VisualContainer();
                    //myContainer.AddVisual(drawingVisual);

                    //add container to the page  
                    pg.Children.Add(grdReport);
                    ((System.Windows.Markup.IAddChild)pageContent).AddChild(pg);
                    Logger.Instance.Log.Debug("after AddChild");


                    //add page to document  
                    fixedDocument.Pages.Add(pageContent);
                    Logger.Instance.Log.Debug("after  fixedDocument.Pages.Add(pageContent);");
                }
                // dialog.PrintVisual(myDocument.DocumentPaginator "Raport");
                Logger.Instance.Log.Debug("before PrintDocument");
                dialog.PrintDocument(fixedDocument.DocumentPaginator, "Raport");
                Logger.Instance.Log.Debug("after PrintDocument");

            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
                Logger.Instance.LogException(ex);

            }
        }

        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.StartPrinting, StartPrinting);
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
