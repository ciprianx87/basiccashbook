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


namespace CashBook.SerialGenerator
{
    /// <summary>
    /// Interaction logic for SerialGeneratorWindow.xaml
    /// </summary>
    public partial class SerialGeneratorWindow : Window, IDisposable
    {
        public SerialGeneratorWindow()
        {
            InitializeComponent();
            this.Loaded += YesNoDialog_Loaded;
        }

        void YesNoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= YesNoDialog_Loaded;
            bool res = this.Activate();
            this.Topmost = true;
            cmbAppType.ItemsSource = new List<string>() { "Registru de casa", "Calcul Impozit" };
            cmbAppType.SelectedIndex = 0;
        }

        public void Dispose()
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //exit the application           
            Exit();
        }

        private void Genereaza_Click(object sender, RoutedEventArgs e)
        {
            string enteredKey = txtSerialNumber.Text;
            CashBook.Common.Utils.ApplicationType appType = cmbAppType.SelectedIndex == 0 ? CashBook.Common.Utils.ApplicationType.CashBook : Common.Utils.ApplicationType.TaxCalculator;
            int pairedKey = CashBook.Common.Utils.GeneratePairedKey(enteredKey, appType);
            txtGeneratedKey.Text = pairedKey.ToString();
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
