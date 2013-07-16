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
using System.Printing;


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
            var lstItems = (sender as PrintedPage).GetLstItems();
            var height = lstItems.ActualHeight;
            int maxRowsPerPage = (int)height / rowHeight;
            (this.DataContext as PrintControlVM).SetMaxEntriesPerPage(maxRowsPerPage);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //PrintAllPages();
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
            try
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
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }

        }

        public void PrintAllPages()
        {
            //grdReport.Children.Clear();
            //Print();
            //set the datacontext for each page and send it to the printer

        }
        public void StartPrinting(object param)
        {
            try
            {
                if (param != null)
                {
                    //grdReport.Children.Clear();
                    List<ReportPageVM> pagesToPrint = param as List<ReportPageVM>;
                    //PrintVmPages(pagesToPrint);
                    PrintVmPagesUpdated(pagesToPrint);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }

        }

        private void PrintVmPages(List<ReportPageVM> pagesToPrint)
        {
            try
            {
                if (pagesToPrint == null || pagesToPrint.Count == 0)
                {
                    return;
                }
                // PrintedPage pp = new PrintedPage();
                PrintDialog dialog = new PrintDialog();
                dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;

                if (dialog.ShowDialog() != true) return;

                foreach (var page in pagesToPrint)
                {
                    var printedPage = new PrintedPage();
                    printedPage.DataContext = page;
                    Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                    grdReport.Children.Clear();
                    grdReport.Children.Add(printedPage);
                    Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                    grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                    grdReport.UpdateLayout();
                    //grdReport.Arrange(new Rect(new Point(50, 50), grdReport.DesiredSize));
                    //grdReport.Visibility = System.Windows.Visibility.Hidden;

                    dialog.PrintVisual(grdReport, "Raport");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        /// <summary>
        /// allows printing multiple pages into the same document, when used in PDF or XPS printing
        /// </summary>
        /// <param name="pagesToPrint"></param>
        private void PrintVmPagesUpdated(List<ReportPageVM> pagesToPrint)
        {
            try
            {
                if (pagesToPrint == null || pagesToPrint.Count == 0)
                {
                    return;
                }
                // PrintedPage pp = new PrintedPage();
                PrintDialog dialog = new PrintDialog();
                dialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                dialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

                if (dialog.ShowDialog() != true) return;

                FixedDocument fixedDocument = new FixedDocument();

                foreach (var page in pagesToPrint)
                {
                    var printedPage = new PrintedPage();
                    printedPage.DataContext = page;
                    Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                    grdReport = new Grid();
                    grdReport.Children.Clear();
                    grdReport.Children.Add(printedPage);
                    Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

                    grdReport.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                    grdReport.UpdateLayout();
                    //grdReport.Arrange(new Rect(new Point(50, 50), grdReport.DesiredSize));
                    //grdReport.Visibility = System.Windows.Visibility.Hidden;

                    // dialog.PrintVisual(grdReport, "Raport");

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


                    //add page to document  
                    fixedDocument.Pages.Add(pageContent);
                }
                // dialog.PrintVisual(myDocument.DocumentPaginator "Raport");
                dialog.PrintDocument(fixedDocument.DocumentPaginator, "Raport");

            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }



        private void PrintVisual(Control element, string description)
        {
            try
            {
                PrintDialog printDlg = new System.Windows.Controls.PrintDialog();

                if (printDlg.ShowDialog() == true)
                {
                    Size szOrg = new Size(element.ActualWidth, element.ActualHeight);
                    System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);
                    double scale = capabilities.PageImageableArea.ExtentWidth / element.ActualWidth;
                    Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, element.ActualHeight);

                    element.Measure(sz);
                    element.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));


                    //Capture the image of the visual in the same size as Printing page.  
                    RenderTargetBitmap bmp = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                    bmp.Render(element);

                    FixedDocument myDocument = new FixedDocument();


                    for (int offset = 0; offset < element.ActualHeight; offset += (int)capabilities.PageImageableArea.ExtentHeight)
                    {
                        DrawingVisual drawingVisual = new DrawingVisual();

                        //create a drawing context so that image can be rendered to print  

                        DrawingContext dc = drawingVisual.RenderOpen();

                        dc.PushTransform(new TranslateTransform(0, -offset));
                        dc.DrawImage(bmp, new System.Windows.Rect(sz));
                        dc.Close();

                        //now print the image visual to printer to fit on the one page.  

                        PageContent pageContent = new PageContent();
                        FixedPage page = new FixedPage();
                        page.Width = capabilities.PageImageableArea.ExtentWidth;
                        page.Height = capabilities.PageImageableArea.ExtentHeight;

                        //container holds our visual...  
                        VisualContainer myContainer = new VisualContainer();
                        myContainer.AddVisual(drawingVisual);

                        //add container to the page  
                        page.Children.Add(myContainer);
                        ((System.Windows.Markup.IAddChild)pageContent).AddChild(page);


                        //add page to document  
                        myDocument.Pages.Add(pageContent);

                    }

                    printDlg.PrintDocument(myDocument.DocumentPaginator, description);


                    element.Measure(szOrg);

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }

        }

        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.StartPrinting, StartPrinting);
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
    public class VisualContainer : FrameworkElement
    {
        private readonly VisualCollection children;

        public VisualContainer()
        {
            children = new VisualCollection(this);
        }

        public void AddVisual(Visual v)
        {
            children.Add(v);
        }

        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }
    }
    public class TestClass
    {
        public string Test { get; set; }
    }
}
