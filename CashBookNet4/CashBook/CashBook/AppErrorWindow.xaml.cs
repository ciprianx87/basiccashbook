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

namespace CashBook
{
    /// <summary>
    /// Interaction logic for AppErrorWindow.xaml
    /// </summary>
    public partial class AppErrorWindow : Window
    {
        string errorMessage;
        public AppErrorWindow(string errorMessage)
        {
            InitializeComponent();
            this.errorMessage = errorMessage;
            this.Loaded += new RoutedEventHandler(AppErrorWindow_Loaded);

        }

        void AppErrorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(AppErrorWindow_Loaded);
            txtErrorMessage.Text = errorMessage;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
               
            }
            catch (Exception ex)
            {
            }
        }
    }
}
