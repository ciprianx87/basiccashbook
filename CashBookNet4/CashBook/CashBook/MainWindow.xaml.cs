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
using System.Windows.Threading;
using CashBook.ViewModels;
using System.Diagnostics;
using System.Management;

namespace CashBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string version = "";

        //<!--<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/>-->
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        private ILog log;


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var x = SystemParameters.VirtualScreenWidth;
            //Configure();
            //DisableWPFTabletSupport();
            //configure log4net
            XmlConfigurator.Configure();
            Logger.Instance.Log.Debug("application start");

            InitCulture();
            InitSettings();
            PopupManager.Instance.Init();
            Mediator.Instance.Register(MediatorActionType.SetMainContent, ChangeContent);
            ShowCashBookListScreen(CashBookListType.Any);
            version = "1.0.21 full";
            //CheckAppValidity();

            CheckAppRegistrationStatus();
            CopyDBIfNeeded();
        }

        private void CopyDBIfNeeded()
        {
            try
            {
                string currentLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                FileInfo currentApp = new FileInfo(currentLocation);
                DirectoryInfo dbDir = currentApp.Directory;// new DirectoryInfo(+"\\Database");
                if (dbDir.Exists)
                {
                    DirectoryInfo emptyDbDir = new DirectoryInfo(dbDir.FullName + "\\Resources");
                    DirectoryInfo databaseDbDir = new DirectoryInfo(dbDir.FullName + "\\Database");
                    FileInfo fi = new FileInfo(databaseDbDir.FullName + "\\CashBook.sdf");
                    if (fi.Exists)
                    {
                    }
                    else
                    {
                        //copy the empty DB

                        FileInfo emptyFi = new FileInfo(emptyDbDir.FullName + "\\CashBookEmpty.sdf");
                        File.Copy(emptyFi.FullName, databaseDbDir.FullName + "\\CashBook.sdf");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        private void CheckAppValidity()
        {
            if (!ConfigureFile())
            {
                WindowHelper.OpenInformationDialog("Perioada de valabilitate a aplicatiei a expirat. Va rugam contactati furnizorul!");
                Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                Thread.Sleep(5000);
                Exit();

            }
        }

        private void CheckAppRegistrationStatus()
        {
            string existingHashKey = GetExistingSerial();
            string motherBoardId = GetMotherBoardID();
            Logger.Instance.Log.Debug("existingHashKey: " + existingHashKey);
            Logger.Instance.Log.Debug("motherBoardId1: " + motherBoardId);
            // string mbId = GetMotherBoardId();
            motherBoardId = motherBoardId.Trim(new char[] { '\\', '/' });
            //remove all the empty chars
            motherBoardId = motherBoardId.Replace(" ", string.Empty);
            Logger.Instance.Log.Debug("motherBoardId2: " + motherBoardId);
            if (motherBoardId.Length > 7)
            {
                Logger.Instance.Log.Debug("motherBoardId length >7: " + motherBoardId);
                motherBoardId = motherBoardId.Substring(0, 7);
                Logger.Instance.Log.Debug("motherBoardId3: " + motherBoardId);
            }
            if (string.IsNullOrEmpty(motherBoardId))
            {
                Logger.Instance.Log.Debug("string.IsNullOrEmpty(motherBoardId) " + motherBoardId);
                //if the serial is empty generate a new one
                //hardcoded 
                motherBoardId = "31T5RBY";
            }
            else
            {
                Logger.Instance.Log.Debug("motherBoardId is not empty " + motherBoardId);
            }
            Logger.Instance.Log.Debug("motherBoardId4: " + motherBoardId);
            int mbHash = Utils.GeneratePairedKey(motherBoardId);
            Logger.Instance.Log.Debug("mbHash: " + mbHash);
            if (string.IsNullOrEmpty(existingHashKey))
            {
                Logger.Instance.Log.Debug("string.IsNullOrEmpty(existingHashKey) true");
                PerformRegistration(motherBoardId, mbHash);
            }
            else
            {
                //verify the serial and the hash key
                int existingKey = 0;
                if (int.TryParse(existingHashKey, out existingKey))
                {
                    Logger.Instance.Log.Debug("existingKey " + existingKey + " mbHash " + mbHash);
                    if (existingKey != mbHash)
                    {
                        MessageBox.Show("Eroare la licentierea aplicatiei.");
                        PerformRegistration(motherBoardId, mbHash);
                        //Thread.Sleep(5000);
                        //Exit();
                    }
                }
                else
                {
                    Logger.Instance.Log.Debug("Eroare la incarcarea informatiilor despre licenta.");
                    MessageBox.Show("Eroare la incarcarea informatiilor despre licenta.");
                    PerformRegistration(motherBoardId, mbHash);
                    //Thread.Sleep(5000);
                    //Exit();
                }

            }
        }

        private void PerformRegistration(string motherBoardId, int mbHash)
        {
            Logger.Instance.Log.Debug("PerformRegistration motherBoardId " + motherBoardId + ", mbHash " + mbHash);
            //prompt the user to enter the validation code
            RegistrationDialog registrationDialog = new RegistrationDialog(motherBoardId, mbHash);
            PopupManager.Instance.CenterPopup(registrationDialog);
            bool? dialogResult = registrationDialog.ShowDialog();
            if (dialogResult == true)
            {
                StreamWriter fileWriter = null;
                try
                {
                    //write the key into the file
                    string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.SerialKeyFilename);
                    FileInfo file = new FileInfo(fileName);
                    fileWriter = new StreamWriter(file.Open(FileMode.Create, FileAccess.Write));
                    fileWriter.WriteLine(mbHash);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException(ex);
                }
                finally
                {
                    try
                    {
                        if (fileWriter != null)
                        {
                            fileWriter.Close();
                        }
                    }
                    catch { }
                }
            }
            else
            {
                Exit();
            }
        }

        private string GetMotherBoardId()
        {
            string serial = "";
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
            foreach (ManagementObject getserial in MOS.Get())
            {
                serial = getserial["SerialNumber"].ToString();
            }
            return serial;
        }

        public static string GetMotherBoardID()
        {
            string mbInfo = String.Empty;
            try
            {
                ManagementScope scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                scope.Connect();
                ManagementObject wmiClass = new ManagementObject(scope, new ManagementPath("Win32_BaseBoard.Tag=\"Base Board\""), new ObjectGetOptions());

                foreach (PropertyData propData in wmiClass.Properties)
                {
                    if (propData.Name == "SerialNumber")
                    {
                        mbInfo = propData.Value.ToString();
                    }
                    //mbInfo = String.Format("{0,-25}{1}", propData.Name, Convert.ToString(propData.Value));
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
            return mbInfo;
        }

        private string GetExistingSerial()
        {
            string serial = "";
            StreamWriter fileWriter = null;
            try
            {
                string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.SerialKeyFilename);
                FileInfo file = new FileInfo(fileName);
                Logger.Instance.Log.Debug("file.Exists: " + file.Exists);
                if (file.Exists)
                {
                    //read current value
                    StreamReader fs = new StreamReader(file.Open(FileMode.Open, FileAccess.Read));
                    serial = fs.ReadLine();
                    return serial;
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
            return serial;

        }
        private bool ConfigureFile()
        {
            StreamWriter fileWriter = null;
            try
            {
                string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.KeyFilename);
                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    //read current value
                    StreamReader fs = new StreamReader(file.Open(FileMode.Open, FileAccess.Read));
                    string[] lines = fs.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    string installDateString = lines[0];
                    string lastOpenedDateString = lines[1];
                    var installDate = DateTime.ParseExact(installDateString, Constants.DateTimeFormatValability, CultureInfo.InvariantCulture);
                    var lastOpenedDate = DateTime.ParseExact(lastOpenedDateString, Constants.DateTimeFormatValability, CultureInfo.InvariantCulture);
                    if (lastOpenedDate > DateTime.Now)
                    {
                        return false;
                    }
                    var elapsedDays = (DateTime.Now - installDate).TotalDays;

                    int remainingDays = Constants.ValabilityDays - (int)elapsedDays;
                    if (remainingDays <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        Constants.RemainingDays = remainingDays;
                        Mediator.Instance.SendMessage(MediatorActionType.SetRemainingDays, remainingDays);
                    }
                    //write the updated values again
                    string currentDate = DateTime.Now.ToString(Constants.DateTimeFormatValability, CultureInfo.InvariantCulture);
                    fs.Close();
                    // fileWriter = file.Open(FileMode.Create, FileAccess.Write);
                    fileWriter = new StreamWriter(file.Open(FileMode.Create, FileAccess.Write));
                    fileWriter.WriteLine(installDateString);

                    //write the last opened date
                    fileWriter.WriteLine(currentDate);
                }
                else
                {
                    fileWriter = file.CreateText();
                    //write the install date
                    string currentDate = DateTime.Now.ToString(Constants.DateTimeFormatValability, CultureInfo.InvariantCulture);
                    fileWriter.WriteLine(currentDate);

                    //write the last opened date
                    fileWriter.WriteLine(currentDate);
                    Constants.RemainingDays = Constants.ValabilityDays;
                    Mediator.Instance.SendMessage(MediatorActionType.SetRemainingDays, Constants.ValabilityDays);
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
            finally
            {
                try
                {
                    if (fileWriter != null)
                    {
                        fileWriter.Close();
                    }
                }
                catch { }
            }
            return true;
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
            //AppSettings.SinglePaymentLimit = Properties.Settings.Default.SinglePaymentLimit;
            //AppSettings.TotalPaymentLimit = Properties.Settings.Default.TotalPaymentLimit;

            Logger.Instance.Log.Debug(string.Format("settings {0}, {1}, {2},{3}", AppSettings.InformationPopupCloseInterval,
                AppSettings.CashRegistryNameCharacterLimit, 0, 0));

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
                case ContentTypes.PrintPreview:
                    contentControl.Content = new CashBook.Controls.Printing.PrintPreview();
                    break;
                case ContentTypes.LegalLimitations:
                    contentControl.Content = new CashBook.Controls.LegalLimitations();
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
            Exit();
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

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string iconInfo = "Icoanele au fost preluate de pe http://www.iconfinder.com/iconsets/finance_icons#readme";
            MessageBox.Show(string.Format("Versiune: {0} {1}{2}{3}", version, Environment.NewLine, Environment.NewLine, iconInfo));

        }

        private void MenuItem_LegalLimitations(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.LegalLimitations);

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
                        Process.Start(Properties.Settings.Default.DocumentationFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nu s-a putut deschide documentul");
                Logger.Instance.LogException(ex);
            }
        }

    }
}
