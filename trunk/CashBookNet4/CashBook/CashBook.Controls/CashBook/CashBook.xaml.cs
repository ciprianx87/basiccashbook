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
using CashBook.ViewModels;
using System.Windows.Controls.Primitives;

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for CashBook.xaml
    /// </summary>
    public partial class CashBook : UserControl, IDisposable
    {
        public CashBook()
        {
            InitializeComponent();
            this.Loaded += CashBook_Loaded;
            this.DataContext = new CashBookViewModel();
            //dataGrid.GotFocus += new RoutedEventHandler(dataGrid_GotFocus);
            //dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            //dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
            //dataGrid.PreparingCellForEdit += dg_PreparingCellForEdit;
            //dataGrid.curre
            //DataGridCell cell = GetCell(rowIndex, colIndex);
            //cell.Focus;

        }

        void dataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as DataGridCell).IsSelected = true;

        }

        private void dg_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            TextBox tb = e.EditingElement as TextBox;
            if (tb != null)
            {
                tb.Focus();
                //you can set caret position and ...
            }
        }
        void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            //DataGrid dg = (DataGrid)sender;
            //dg.BeginEdit();
        }

        void CashBook_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CashBook_Loaded;
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void dataGrid_KeyUp(object sender, KeyEventArgs e)
        {       
            if (e.Key == Key.Enter)
            {
                (this.DataContext as CashBookViewModel).AddNewItem();
            }
            if (e.Key == Key.Right)
            {
                var currCell = dataGrid.CurrentCell;
                //var cell = TryToFindGridCell(dataGrid, currCell);
            }
        }

        private void DataGridCell_Selected(object sender, RoutedEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                    cell.Focus();

                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                            row.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                            row.IsSelected = true;
                    }
                }
                if (cell.IsFocused)
                {
                    try
                    {
                        var cp = cell.Content as ContentPresenter;
                        var ct = cp.ContentTemplate;
                        this.ApplyTemplate();
                        cp.ApplyTemplate();
                        var textbox = ct.FindName("txtContent", cp) as TextBox;
                        if (textbox != null)
                        {
                            SelectContentTextBox(textbox);
                        }
                    }
                    catch { }
                }
                //    cell.IsEditing = true;
            }
        }



        private void SelectContentTextBox(TextBox txtBox)
        {
            //txtBox.IsFocused = true;
            txtBox.Focus();
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                    return correctlyTyped;
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }
}
