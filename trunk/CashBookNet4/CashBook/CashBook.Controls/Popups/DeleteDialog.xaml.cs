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
    /// Interaction logic for DeleteDialog.xaml
    /// </summary>
    public partial class DeleteDialog : Window
    {
        public DeleteDialog()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DeleteDialog_Loaded);
        }

        void DeleteDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(DeleteDialog_Loaded);
            this.DataContext = new DeleteDialogVM();

            //set in the center of the screen
            double parentX = Application.Current.MainWindow.Left;
            double parentY = Application.Current.MainWindow.Top;
            double parentWidth = Application.Current.MainWindow.Width;
            double parentHeight = Application.Current.MainWindow.Height;

            this.Left = parentX + (parentWidth - this.Width) / 2;
            this.Top = parentY + (parentHeight - this.Height) / 2;
        }
    }
}
