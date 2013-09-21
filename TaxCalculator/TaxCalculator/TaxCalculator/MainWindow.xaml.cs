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
using log4net.Config;
using TaxCalculator.Controls.Popups;
using TaxCalculator.Common.Mediator;
using TaxCalculator.Data.Model;
using Microsoft.Win32;
using System.Diagnostics;
using TaxCalculator.Common;

namespace TaxCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string version;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            XmlConfigurator.Configure();
            Logger.Instance.Log.Debug("application start");

            InitCulture();
            InitSettings();
            PopupManager.Instance.Init();
            //Mediator.Instance.Register(MediatorActionType.SetMainContent, ChangeContent);
            ChangeContent(ContentTypes.CompanyList);
            version = "1.0.22 demo";
            CheckAppValidity();

            //CheckAppRegistrationStatus();
            CopyDBIfNeeded();
        }
        private void ShowCashBookListScreen(CashBookListType type)
        {
            ChangeContent(ContentTypes.CashBookList);
            Mediator.Instance.SendMessage(MediatorActionType.SetCashBookListType, type);
        }
        private void ChangeContent(object contentType)
        {
            DisposeCurrentContent();
            ContentTypes type = (ContentTypes)contentType;
            switch (type)
            {
                case ContentTypes.TaxIndicatorList:
                    contentControl.Content = new TaxCalculator.Controls.TaxIndicatorsList();
                    break;
                case ContentTypes.TaxCalculation:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculation();
                    break;
                case ContentTypes.TaxCalculationTest:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculationTest();
                    break;
                case ContentTypes.CompanyList:
                    contentControl.Content = new TaxCalculator.Controls.CompanyList();
                    break;
                //case ContentTypes.MainContent:
                //    //contentControl.Content = new CashBook.Controls.MainContent();
                //    break;
                //case ContentTypes.CompanyDetails:
                //    contentControl.Content = new CashBook.Controls.CompanyDataWindow();
                //    break;
                //case ContentTypes.LegalReglementations:
                //    contentControl.Content = new CashBook.Controls.LegalReglementations();
                //    break;
                //case ContentTypes.CashBookList:
                //    contentControl.Content = new CashBook.Controls.CashBookList();
                //    break;
                //case ContentTypes.Reports:
                //    contentControl.Content = new Report();
                //    break;
                //case ContentTypes.PrintControl:
                //    contentControl.Content = new CashBook.Controls.PrintControl();
                //    break;
                //case ContentTypes.PrintPreview:
                //    contentControl.Content = new CashBook.Controls.Printing.PrintPreview();
                //    break;
                //case ContentTypes.LegalLimitations:
                //    contentControl.Content = new CashBook.Controls.LegalLimitations();
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

        private void InitSettings()
        {
            AppSettings.InformationPopupCloseInterval = 5000;
        }

        private void InitCulture()
        {

        }

        private void CopyDBIfNeeded()
        {
            DbHelper.CopyDBIfNeeded("TaxCalculatorEmpty.sdf", "TaxCalculator.sdf", "Resources", "Database");                
        }

        private void CheckAppValidity()
        {

        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var regWord = Registry.ClassesRoot.OpenSubKey("Word.Application"))
                {
                    if (regWord == null)
                    {
                        MessageBox.Show("Trebuie sa aveti instalat Office Word pentru a putea deschide acest document");
                    }
                    else
                    {
                        //Process.Start(Properties.Settings.Default.DocumentationFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nu s-a putut deschide documentul");
                Logger.Instance.LogException(ex);
            }
        }

        private void MenuItem_Iesire(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.CompanyList);
        }

        private void MenuItem_Companies(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_TaxIndicators(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.TaxIndicatorList);

        }

        private void MenuItem_TaxCalculation(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.TaxCalculation);

        }

        private void MenuItem_Reports(object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.TaxCalculationTest);
            
        }

        private void TestDefault_Click(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.TaxCalculation);
        }

    }
}
