using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;

namespace CashBook.Controls.CustomDataGrid
{
    public class CustomDataGrid1 : DataGrid
    {
        public CustomDataGrid1()
        {
            this.PreviewKeyDown += CustomDataGrid_PreviewKeyDown;
            this.Loaded += CustomDataGrid_Loaded;
        }
        void CustomDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShowToggleButton)
                this.AddToggleButtonForDetailView();
        }
        #region Toggle Button Column

        private void AddToggleButtonForDetailView()
        {
            DataGridTemplateColumn theColumn = new DataGridTemplateColumn();
            theColumn.Header = "";
            FrameworkElementFactory theFactory = new FrameworkElementFactory(typeof(ToggleButton));
            #region Set Binding

            //Binding b1 = new Binding("IsSingleTabFixed");
            //b1.Mode = BindingMode.TwoWay;
            //theFactory.SetValue(CheckBox.IsCheckedProperty, b1);
            #endregion End Set Binding
            //Set Style to dynamic source Resources/Style.xaml
            theFactory.SetResourceReference(StyleProperty, "TwistyButtonStyle");
            theFactory.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler(ToggleButton_Click));
            theColumn.CellTemplate = new DataTemplate()
            {
                VisualTree = theFactory
            };
            theColumn.DisplayIndex = 0;
            this.Columns.Add(theColumn);
        }
        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //Show Detail row
            DataGridRow theSelectedRow = this.GetSelectedRow();
            ToggleButton theToggleButton = sender as ToggleButton;
            if (theToggleButton.IsChecked != null && theToggleButton.IsChecked == true)
                theSelectedRow.DetailsVisibility = Visibility.Visible;
            else
                theSelectedRow.DetailsVisibility = Visibility.Collapsed;
            //Execute pass in Action - To refesh UI DocLinkControlList
            this.ToggleOutSideAction.Invoke();
        }
        #endregion End Toggle Button Column
        void CustomDataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                //One Tab focus cell fix
                if (this.IsSingleTabFixed)
                    this.FocusNextCell(this);
            }
        }
        /// <summary>
        /// Focus next cell based on Keyboard.Modifiers
        /// </summary>
        /// <param name="theGrid"></param>
        private void FocusNextCell(CustomDataGrid1 theGrid)
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



        #region Dependency Properties
        public static readonly DependencyProperty IsSingleTabFixedProperty = DependencyProperty.Register(
         "IsSingleTabFixed", //Property name
         typeof(bool), //Property type
         typeof(CustomDataGrid1), //Type of the dependency property provider (Whatever class it's in now)
         new PropertyMetadata(IsSingleTabFixedChanged) //Callback invoked on property value has changes
        );
        public bool IsSingleTabFixed
        {
            set { this.SetValue(IsSingleTabFixedProperty, value); }
            get { return (bool)this.GetValue(IsSingleTabFixedProperty); }
        }
        static void IsSingleTabFixedChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            //MessageBox.Show(sender.GetType().Name + " " +
            //     args.NewValue.ToString() + " " +
            //         ((args.OldValue == null) ? "" : args.OldValue).ToString());
            //Do some processing here...
        }
        public static readonly DependencyProperty ShowToggleButtonProperty = DependencyProperty.Register(
         "ShowToggleButton", //Property name
         typeof(bool), //Property type
         typeof(CustomDataGrid1), //Type of the dependency property provider (Whatever class it's in now)
         new PropertyMetadata(ShowToggleButtonChanged) //Callback invoked on property value has changes
        );



        public bool ShowToggleButton
        {
            set { this.SetValue(ShowToggleButtonProperty, value); }
            get { return (bool)this.GetValue(ShowToggleButtonProperty); }
        }
        static void ShowToggleButtonChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            //MessageBox.Show(sender.GetType().Name + " " +
            //     args.NewValue.ToString() + " " +
            //         ((args.OldValue == null) ? "" : args.OldValue).ToString());
            //Do some processing here...
        }
        public static readonly DependencyProperty ToggleOutSideActionProperty = DependencyProperty.Register(
         "ToggleOutSideAction", //Property name
         typeof(Action), //Property type
         typeof(CustomDataGrid1), //Type of the dependency property provider (Whatever class it's in now)
         new PropertyMetadata(ToggleOutSideActionChanged) //Callback invoked on property value has changes
        );
        public Action ToggleOutSideAction
        {
            set { this.SetValue(ToggleOutSideActionProperty, value); }
            get { return (Action)this.GetValue(ToggleOutSideActionProperty); }
        }
        static void ToggleOutSideActionChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            //MessageBox.Show(sender.GetType().Name + " " +
            //     args.NewValue.ToString() + " " +
            //         ((args.OldValue == null) ? "" : args.OldValue).ToString());
            //Do some processing here...
        }
        #endregion Dependency Properties
    }
}
