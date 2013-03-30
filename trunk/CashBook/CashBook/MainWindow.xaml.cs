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

namespace CashBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeContent(ContentTypes.CashBook);
            
        }

        private void ChangeContent(object contentType)
        {
            ContentTypes type = (ContentTypes)contentType;
            switch (type)
            {
                case ContentTypes.CashBook:
                    contentControl.Content = new CashBook.Controls.CashBookWindow();
                    break;
                case ContentTypes.MainContent:
                    contentControl.Content = new CashBook.Controls.MainContent();
                    break;
                default:
                    break;
            }
        }
    }
}
