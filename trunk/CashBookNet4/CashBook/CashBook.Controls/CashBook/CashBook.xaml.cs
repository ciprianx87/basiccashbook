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
using System.Threading;
using CashBook.Controls.CustomDataGrid;


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
            //dataGrid.CurrentCellChanged+=new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            dataGrid.KeyUp += DataGridSubmissionDataOnKeyUp;
            dataGrid.BeginningEdit += DataGridSubmissionDataOnBeginningEdit;
            dataGrid.CellEditEnding += DataGridSubmissionDataOnCellEditEnding;
            dataGrid.CurrentCellChanged += DataGridSubmissionDataOnCurrentCellChanged;

        }

        private void NumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            txtBox.SelectAll();
        }
        private bool isCellInEditionMode = false;

        private void DataGridSubmissionDataOnCurrentCellChanged(object sender, EventArgs eventArgs)
        {
           bool result= dataGrid.BeginEdit();
           if (!result)
           {
               //this.Dispatcher.BeginInvoke(new Action(() =>
               //{
               //    result = dataGrid.BeginEdit();
               //    dataGrid.UpdateLayout();
               //}), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);
             
              
           }
        }

        private void DataGridSubmissionDataOnCellEditEnding(object sender, DataGridCellEditEndingEventArgs dataGridCellEditEndingEventArgs)
        {
            isCellInEditionMode = false;
        }

        private void DataGridSubmissionDataOnBeginningEdit(object sender, DataGridBeginningEditEventArgs dataGridBeginningEditEventArgs)
        {
            isCellInEditionMode = true;
        }

        private void DataGridSubmissionDataOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Tab)
            {
                this.FocusNextCell(dataGrid);
                return;
            }

            //Thread.Sleep(100);
            if (keyEventArgs.Key == Key.Up || keyEventArgs.Key == Key.Down || keyEventArgs.Key == Key.Left || keyEventArgs.Key == Key.Right)
            {
                if (!isCellInEditionMode)
                    return;


                bool result = dataGrid.CommitEdit();
                if (!result)
                {
                    dataGrid.UpdateLayout();
                    result = dataGrid.CommitEdit();
                }

                //DataGridCell cell = GetCell(1, 0);
                //if (cell != null)
                //{
                //    cell.Focus();
                //    dataGrid.BeginEdit();
                //}

                var key = keyEventArgs.Key; // Key to send
                var target = dataGrid; // Target element
                var routedEvent = Keyboard.KeyDownEvent; // Event to send

                target.RaiseEvent(
                    new KeyEventArgs(
                        Keyboard.PrimaryDevice,
                        PresentationSource.FromVisual(target),
                        0,
                        key) { RoutedEvent = routedEvent }
                    );
            }
        }

        private void FocusNextCell(DataGrid theGrid)
        {
            DataGridRow currentRow = theGrid.GetSelectedRow();
            int currentColumnIndex = theGrid.CurrentCell.Column.DisplayIndex;
            int nextColumnIndex;
            //If Shift is hold, then reverse, else move forward
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                nextColumnIndex = currentColumnIndex;
            else
                nextColumnIndex = currentColumnIndex + 1;
            DataGridCell theNextCell = null;
            if (theGrid.Columns.Count > nextColumnIndex &&
             nextColumnIndex >= 0)
                theNextCell = theGrid.GetCell(currentRow, nextColumnIndex);
            if (theNextCell != null && theNextCell.Focusable)
                theNextCell.Focus();
        }

        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
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
            return;
            if (e.Key == Key.Right)
            {
                var currCell = dataGrid.CurrentCell;
                //Move2(currCell, e);
                //currCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                //var cell = TryToFindGridCell(dataGrid, currCell);
            }
        }

        private void DataGridCell_Selected(object sender, RoutedEventArgs e)
        {
            return;
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
