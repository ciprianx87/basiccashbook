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
            this.DialogResult = true;
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
