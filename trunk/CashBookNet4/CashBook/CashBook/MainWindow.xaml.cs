using CashBook.Data.Model;
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
using CashBook.Common.Mediator;
using CashBook.Controls.Popups;
using CashBook.Common;
using CashBook.Controls;

namespace CashBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }



        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitSettings();
            PopupManager.Instance.Init();
            Mediator.Instance.Register(MediatorActionType.SetMainContent, ChangeContent);
            ShowCashBookListScreen(CashBookListType.Any);
        }

        private void InitSettings()
        {
            AppSettings.InformationPopupCloseInterval = Properties.Settings.Default.InformationPopupCloseInterval;
        }

        private void InitErrorHandling()
        {
            Dispatcher.UnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Dispatcher_UnhandledException);
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowError(e.Exception);
        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowError(e.Exception);
        }
        private void ShowError(Exception ex)
        {
            try
            {
                string errorMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                }
                AppErrorWindow errorWindow = new AppErrorWindow(errorMessage);
                errorWindow.Show();
                Application.Current.Shutdown();

            }
            catch (Exception exc)
            {
            }
        }
        private void ChangeContent(object contentType)
        {
            DisposeCurrentContent();
            ContentTypes type = (ContentTypes)contentType;
            switch (type)
            {
                case ContentTypes.CashBook:
                    contentControl.Content = new CashBook.Controls.CashBook();
                    break;
                case ContentTypes.MainContent:
                    //contentControl.Content = new CashBook.Controls.MainContent();
                    break;
                case ContentTypes.CompanyDetails:
                    contentControl.Content = new CashBook.Controls.CompanyDataWindow();
                    break;
                case ContentTypes.LegalReglementations:
                    contentControl.Content = new CashBook.Controls.LegalReglementations();
                    break;
                case ContentTypes.CashBookList:
                    contentControl.Content = new CashBook.Controls.CashBookList();
                    break;
                case ContentTypes.Reports:
                    contentControl.Content = new Report();
                    break;
                //case ContentTypes.CreateCashBook:
                //    //contentControl.Content = new CashBook.Controls.CreateCashBook();
                //    break;
                default:
                    MessageBox.Show("invalid contentType: " + type);
                    break;

            }
            //var title = ((CashBook.ViewModels.BaseViewModel)((UserControl)contentControl.Content).DataContext).Title;
            //var binding = new Binding("Title");
            this.DataContext = ((UserControl)contentControl.Content).DataContext;

            //this.SetBinding(Window.TitleProperty, binding);
        }

        private void DisposeCurrentContent()
        {
            if (contentControl.Content != null)
            {
                (contentControl.Content as IDisposable).Dispose();
            }
        }

        private void MenuItem_CompanyData(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.CompanyDetails);
        }

        private void MenuItem_CashBooks(object sender, RoutedEventArgs e)
        {
            ShowCashBookListScreen(CashBookListType.Any);
        }

        private void MenuItem_LegalReglementations(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.LegalReglementations);
        }

        private void MenuItem_CashBooksOther(object sender, RoutedEventArgs e)
        {
            ShowCashBookListScreen(CashBookListType.Other);
        }

        private void MenuItem_CashBooksLei(object sender, RoutedEventArgs e)
        {
            ShowCashBookListScreen(CashBookListType.Lei);
        }

        private void ShowCashBookListScreen(CashBookListType type)
        {
            ChangeContent(ContentTypes.CashBookList);
            Mediator.Instance.SendMessage(MediatorActionType.SetCashBookListType, type);
        }

    }
}
