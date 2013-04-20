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
                Move2(currCell, e);
                //currCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                //var cell = TryToFindGridCell(dataGrid, currCell);
            }
        }
        private void Move2(DataGridCellInfo currentCell, KeyEventArgs e)
        {
            bool previous = false;
            FocusNavigationDirection direction = previous ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
            TraversalRequest request = new TraversalRequest(direction);
            request.Wrapped = true;
            var selectedCell = dataGrid.SelectedCells[0];
            //dataGrid.MoveFocus(request);
        }

        private void Move(DataGridCellInfo currentCell, KeyEventArgs e)
        {
            // get the display index of the cell's column + 1 (for next column)
            int columnDisplayIndex = currentCell.Column.DisplayIndex++;

            // if display index is valid
            if (columnDisplayIndex < dataGrid.Columns.Count)
            {
                // get the DataGridColumn instance from the display index
                DataGridColumn nextColumn = dataGrid.ColumnFromDisplayIndex(columnDisplayIndex);

                // now telling the grid, that we handled the key down event
                e.Handled = true;
             
                // setting the current cell (selected, focused)
                //dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.SelectedItem, nextColumn);

                // tell the grid to initialize edit mode for the current cell
                //dataGrid.BeginEdit();
            }
        }
        /*
        private void OnTabKeyDown(KeyEventArgs e)
        {
            // When the end-user uses the keyboard to tab to another cell while the current cell
            // is in edit-mode, then the next cell should enter edit mode in addition to gaining
            // focus. There is no way to detect this from the focus change events, so the cell
            // is going to handle the complete operation manually.
            // The standard focus change method is being called here, so even if focus moves
            // to something other than a cell, focus should land on the element that it would
            // have landed on anyway.
            DataGridCell currentCellContainer = CurrentCellContainer;
            if (currentCellContainer != null)
            {
                bool wasEditing = currentCellContainer.IsEditing;
                bool previous = ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);

                // Start navigation from the current focus to allow moveing focus on other focusable elements inside the cell
                UIElement startElement = Keyboard.FocusedElement as UIElement;
                ContentElement startContentElement = (startElement == null) ? Keyboard.FocusedElement as ContentElement : null;
                if ((startElement != null) || (startContentElement != null))
                {
                    e.Handled = true;

                    FocusNavigationDirection direction = previous ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                    TraversalRequest request = new TraversalRequest(direction);
                    request.Wrapped = true; // Navigate only within datagrid

                    // Move focus to the the next or previous tab stop.
                    if (((startElement != null) && startElement.MoveFocus(request)) ||
                        ((startContentElement != null) && startContentElement.MoveFocus(request)))
                    {
                        // If focus moved to the cell while in edit mode - keep navigating to the previous cell
                        if (wasEditing && previous && Keyboard.FocusedElement == currentCellContainer)
                        {
                            currentCellContainer.MoveFocus(request);
                        }

                        // In case of grouping if a row level commit happened due to
                        // the previous focus change, the container of the row gets
                        // removed from the visual tree by the CollectionView, 
                        // but we still hang on to a cell of that row, which will be used
                        // by the call to SelectAndEditOnFocusMove. Hence re-establishing the
                        // focus appropriately in such cases.
                        if (IsGrouping && wasEditing)
                        {
                            DataGridCell newCell = GetCellForSelectAndEditOnFocusMove();

                            if (newCell != null &&
                                newCell.RowDataItem == currentCellContainer.RowDataItem)
                            {
                                DataGridCell realNewCell = TryFindCell(newCell.RowDataItem, newCell.Column);

                                // Forcing an UpdateLayout since the generation of the new row
                                // container which was removed earlier is done in measure.
                                if (realNewCell == null)
                                {
                                    UpdateLayout();
                                    realNewCell = TryFindCell(newCell.RowDataItem, newCell.Column);
                                }
                                if (realNewCell != null && realNewCell != newCell)
                                {
                                    realNewCell.Focus();
                                }
                            }
                        }

                        // When doing TAB and SHIFT+TAB focus movement, don't confuse the selection
                        // code, which also relies on SHIFT to know whether to extend selection or not.
                        //SelectAndEditOnFocusMove(e, currentCellContainer, wasEditing, allowsExtendSelect =  false, ignoreControlKey =  true);
                    }
                }
            }
        }
*/
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
