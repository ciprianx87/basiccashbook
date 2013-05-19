using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using CashBook.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CashBook.Common;
using System.Windows.Threading;

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for CashBookList.xaml
    /// </summary>
    public partial class CashBookList : UserControl, IDisposable
    {

        public CashBookList()
        {
            InitializeComponent();
            this.Loaded += CashBookList_Loaded;
            this.DataContext = new CashBookListViewModel();
            Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
        }

        void CashBookList_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CashBookList_Loaded;
        }



        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Instance.Log.Debug("select button pressed");
            Logger.Instance.Log.Debug(string.Format(" CommandParameter!=null {0}", (sender as Button).CommandParameter != null));
            Logger.Instance.Log.Debug(string.Format(" Command!=null {0}", (sender as Button).Command != null));
            Logger.Instance.Log.Debug(string.Format(" VMCommand!=null {0}", (this.DataContext as CashBookListViewModel).SelectCommand != null));
            var but = (sender as Button);
            (this.DataContext as CashBookListViewModel).Select(but.CommandParameter);

            //if (but.Command != null)
            //{
            //    but.Command.Execute(but.CommandParameter);
            //}
        }
        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as CashBookListViewModel).Delete(but.CommandParameter);
        }
        private void EditCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as CashBookListViewModel).Edit(but.CommandParameter);
        }

    }
}
