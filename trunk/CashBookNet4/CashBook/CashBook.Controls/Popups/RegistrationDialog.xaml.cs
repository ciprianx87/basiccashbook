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
using System.Windows.Shapes;
using CashBook.ViewModels;
using CashBook.Common;


namespace CashBook.Controls.Popups
{
    /// <summary>
    /// Interaction logic for RegistrationDialog.xaml
    /// </summary>
    public partial class RegistrationDialog : Window, IDisposable
    {
        string motherBoardId;
        int expectedSerial;
        public RegistrationDialog(string motherBoardId, int expectedSerial)
        {
            InitializeComponent();
            Logger.Instance.Log.Debug("RegistrationDialog motherBoardId " + motherBoardId + ", mbHash " + expectedSerial);
            this.motherBoardId = motherBoardId;
            this.expectedSerial = expectedSerial;
            //this.DataContext = new YesNoDialogVM();
            this.Loaded += YesNoDialog_Loaded;
        }

        void YesNoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= YesNoDialog_Loaded;
            bool res = this.Activate();
            this.Topmost = true;
            txtGeneratedKey.Text = motherBoardId;
            txtInformation.ToolTip = motherBoardId;
            Logger.Instance.Log.Debug("YesNoDialog_Loaded set value to txtGeneratedKey.Text:" + txtGeneratedKey.Text);
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //exit the application
            this.DialogResult = false;
            //Exit();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string enteredKey = txtSerialNumber.Text;
            Logger.Instance.Log.Debug("Ok_Click enteredKey:" + enteredKey + " expectedSerial " + expectedSerial);
            if (enteredKey == expectedSerial.ToString())
            {
                this.DialogResult = true;
            }
            else
            {
                Logger.Instance.Log.Debug("Ok_Click Codul introdus nu este valid!");
                MessageBox.Show("Codul introdus nu este valid!");
            }
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

    }
}
