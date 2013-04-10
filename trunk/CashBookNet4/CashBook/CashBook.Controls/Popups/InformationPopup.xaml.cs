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

namespace CashBook.Controls.Popups
{
    /// <summary>
    /// Interaction logic for SettingsSavedPopup.xaml
    /// </summary>
    public partial class InformationPopup : Window
    {
        public InformationPopup()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(InformationPopup_Loaded);
           
        }

        void InformationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(InformationPopup_Loaded);
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public void SetMessage(string msg)
        {
            txtMessage.Text = msg;
        }
    }
}
