using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TaxCalculator.Common;
using TaxCalculator.Common.Mediator;
using System.Windows;
using TaxCalculator.Controls.Popups;
using System.Windows.Threading;
using System.Threading;
using System.Management;
using System.Globalization;

namespace TaxCalculator
{
    public static class AppHelpers
    {
        public static void CheckApplicationValidity(bool isFullVersion, Dispatcher dispatcher)
        {
            if (isFullVersion)
            {
                CheckAppRegistrationStatus();
            }
            else
            {
                CheckAppValidity(dispatcher);
            }
        }

        public static void CheckAppRegistrationStatus()
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
            //Logger.Instance.Log.Debug("mbHash: " + mbHash);
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
                    //Logger.Instance.Log.Debug("existingKey " + existingKey + " mbHash " + mbHash);
                    Logger.Instance.Log.Debug("existingKey " + existingKey);
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

        public static void PerformRegistration(string motherBoardId, int mbHash)
        {
            //Logger.Instance.Log.Debug("PerformRegistration motherBoardId " + motherBoardId + ", mbHash " + mbHash);
            Logger.Instance.Log.Debug("PerformRegistration motherBoardId " + motherBoardId);
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

        public static string GetMotherBoardId()
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

        private static string GetExistingSerial()
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

        public static void CheckAppValidity(Dispatcher dispatcher)
        {
            if (!ConfigureFile())
            {
                WindowHelper.OpenInformationDialog("Perioada de valabilitate a aplicatiei a expirat. Va rugam contactati furnizorul!");
                dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                Thread.Sleep(5000);
                Exit();

            }
        }
        public static bool ConfigureFile()
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
        public static void Exit()
        {
            try
            {
                Application.Current.Shutdown();
            }
            catch
            {
            }
        }
    }
}
