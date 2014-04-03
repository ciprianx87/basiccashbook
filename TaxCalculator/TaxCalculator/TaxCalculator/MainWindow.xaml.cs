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
using System.IO;
using TaxCalculator.Data;
using System.Threading;
using System.Windows.Threading;

namespace TaxCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isFullVersion;
        private string version;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            isFullVersion = true;
            version = "1.0.6 full";
            //load settings
            Constants.DocumentationFileName = Properties.Settings.Default.DocumentationFileName;
            Constants.RulesFileName = Properties.Settings.Default.RulesFileName;
            XmlConfigurator.Configure();
            Logger.Instance.Log.Debug("application start");

            InitCulture();
            InitSettings();
            PopupManager.Instance.Init();
            Mediator.Instance.Register(MediatorActionType.SetMainContent, ChangeContent);

            CopyDBIfNeeded();

            ChangeContent(ContentTypes.TaxCalculationList);
            AppHelpers.CheckApplicationValidity(isFullVersion, Dispatcher);

            //CheckAppRegistrationStatus();
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
                //case ContentTypes.TaxCalculation:
                //    contentControl.Content = new TaxCalculator.Controls.TaxCalculation();
                //    break;
                case ContentTypes.TaxCalculationTest:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculationTest();
                    break;
                case ContentTypes.CompanyList:
                    contentControl.Content = new TaxCalculator.Controls.CompanyList();
                    break;
                case ContentTypes.EditIndicators:
                    contentControl.Content = new TaxCalculator.Controls.EditIndicators();
                    break;
                case ContentTypes.TaxCalculationSetup:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculationSetup();
                    break;
                case ContentTypes.TaxCalculationCompletion:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculationCompletion();
                    break;
                case ContentTypes.ExistingTaxCalculationCompletion:
                    contentControl.Content = new TaxCalculator.Controls.ExistingTaxCalculationCompletion();
                    break;
                case ContentTypes.TaxCalculationList:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculationList(false);
                    break;
                case ContentTypes.TaxCalculationListRectifying:
                    contentControl.Content = new TaxCalculator.Controls.TaxCalculationList(true);
                    break;
                case ContentTypes.PrintPreview:
                    contentControl.Content = new TaxCalculator.Controls.Printing.PrintControl();
                    break;
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
            AppSettings.InformationPopupCloseInterval = 1500;
            DecimalConvertor.Instance.SetNumberOfDecimals(2);
            Constants.RulesText = RulesHelper.GetRulesText();
        }

        private void InitCulture()
        {

        }

        private void CopyDBIfNeeded()
        {
            DbHelper.CopyDBIfNeeded("TaxCalculatorEmpty.sdf", "TaxCalculator.sdf", "Resources", "Database");
        }



        private void Help_Click(object sender, RoutedEventArgs e)
        {
            OpenWordDocument(Constants.DocumentationFileName);
        }

        private static void OpenWordDocument(string fileName)
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
                        Process.Start(fileName);
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
            Exit();
        }

        private void MenuItem_Companies(object sender, RoutedEventArgs e)
        {
            HighlightMenu(menuCatalogs);
            ChangeContent(ContentTypes.CompanyList);

        }

        private void MenuItem_TaxIndicators(object sender, RoutedEventArgs e)
        {
            HighlightMenu(menuCatalogs);
            ChangeContent(ContentTypes.TaxIndicatorList);

        }

        private void MenuItem_TaxCalculation(object sender, RoutedEventArgs e)
        {
            HighlightMenu(menuTaxCalculation);
            ChangeContent(ContentTypes.TaxCalculationSetup);

        }

        private void MenuItem_Reports(object sender, RoutedEventArgs e)
        {

        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.TaxCalculationTest);

        }

        private void TestDefault_Click(object sender, RoutedEventArgs e)
        {
            //ChangeContent(ContentTypes.TaxCalculation);
        }
        private void Exit()
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch
            {
            }
        }

        private void Report_TaxCalculation_Click(object sender, RoutedEventArgs e)
        {
            HighlightMenu(menuReports);
            ChangeContent(ContentTypes.TaxCalculationList);

        }

        private void Report_TaxCalculation_Rectifying_Click(object sender, RoutedEventArgs e)
        {
            HighlightMenu(menuReports);
            ChangeContent(ContentTypes.TaxCalculationListRectifying);

        }
        private void HighlightMenu(MenuItem menuItem)
        {
            ClearHighlights();
            menuItem.Background = new SolidColorBrush(Colors.Gray);
        }
        private void ClearHighlights()
        {
            var menuBrush = new SolidColorBrush(Colors.Transparent);
            menuCatalogs.Background = menuBrush;
            menuReports.Background = menuBrush;
            menuTaxCalculation.Background = menuBrush;
        }

        private void Rules_Click(object sender, RoutedEventArgs e)
        {
            OpenWordDocument(Constants.RulesFileName);
            //Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.Rules);
            //Mediator.Instance.SendMessage(MediatorActionType.SetInformationPopupMessage, Constants.RulesText);

        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            string iconInfo = "Icoanele au fost preluate de pe http://www.iconarchive.com/show/3d-cartoon-icons-by-hopstarter/Axialis-Icon-Workshop-icon.html";
            MessageBox.Show(string.Format("Versiune: {0} {1}{2}{3}", version, Environment.NewLine, Environment.NewLine, iconInfo));

        }
    }
}
