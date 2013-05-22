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
using System.Globalization;
using System.Threading;
using log4net.Config;
using log4net;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace CashBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //<!--<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>-->
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        private ILog log;


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigureFile();
            //Configure();
            //DisableWPFTabletSupport();
            //configure log4net
            XmlConfigurator.Configure();
            //log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //log.Debug("test");
            //log.Error("test");
            Logger.Instance.Log.Debug("application start");

            InitCulture();
            InitSettings();
            PopupManager.Instance.Init();
            Mediator.Instance.Register(MediatorActionType.SetMainContent, ChangeContent);
            ShowCashBookListScreen(CashBookListType.Any);
        }
        private static bool checkIfKeyExists(Microsoft.Win32.RegistryKey subKey)
        {
            bool status = true;
            if (subKey == null)
            {
                status = false;
            }
            return status;
        }
        private void ConfigureFile()
        {
            StreamWriter namewriter = null;
            try
            {
                string fileName = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "userinfo.txt");
                FileInfo fileusername = new FileInfo(fileName);
                 namewriter = fileusername.CreateText();
                namewriter.Write("test");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
            finally
            {
                try
                {
                    namewriter.Close();
                }
                catch { }
            }
        }
        private void Configure()
        {
            //http://tutplusplus.blogspot.ro/2010/10/c-tutorial-create-and-delete-registry.html
            try
            {
                Microsoft.Win32.RegistryKey sk =
Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Office");
                if (checkIfKeyExists(sk))
                {
                    sk =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Office\\11.0");
                    if (checkIfKeyExists(sk))
                    {
                        sk =
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Office\\11.0\\Outlook\\Options");
                        if (!checkIfKeyExists(sk))
                        {
                            Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Office\\11.0");
                        }
                    }
                }

                string subKeyName = "CashBook";
                string keyName = "InstallDate";
                var swKey = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                var existingSubKey = swKey.OpenSubKey(subKeyName, true);
                if (existingSubKey == null)
                {
                    var subKey = swKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
                existingSubKey = swKey.OpenSubKey(subKeyName, true);
                var result = existingSubKey.GetValue(keyName, null);
                if (result != null)
                {
                    existingSubKey.DeleteValue(keyName);
                }
                else
                {
                    existingSubKey.SetValue(keyName, "test1");
                    result = existingSubKey.GetValue(keyName, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
        private void InitCulture()
        {
            CultureInfo ci = new CultureInfo("ro-RO");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private void InitSettings()
        {
            AppSettings.InformationPopupCloseInterval = Properties.Settings.Default.InformationPopupCloseInterval;
            AppSettings.CashRegistryNameCharacterLimit = Properties.Settings.Default.CashRegistryNameCharacterLimit;
            AppSettings.SinglePaymentLimit = Properties.Settings.Default.SinglePaymentLimit;
            AppSettings.TotalPaymentLimit = Properties.Settings.Default.TotalPaymentLimit;

            Logger.Instance.Log.Debug(string.Format("settings {0}, {1}, {2},{3}", AppSettings.InformationPopupCloseInterval,
                AppSettings.CashRegistryNameCharacterLimit, AppSettings.SinglePaymentLimit, AppSettings.TotalPaymentLimit));

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
                Logger.Instance.LogException(ex);
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
                case ContentTypes.PrintControl:
                    contentControl.Content = new CashBook.Controls.PrintControl();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Iesire(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.  
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            //if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    //while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                                null, stylusLogic, new object[] { (uint)0 });
                    }
                }

            }
        }

        private void MenuItem_Reports(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.PrintControl);

        }

    }



}
