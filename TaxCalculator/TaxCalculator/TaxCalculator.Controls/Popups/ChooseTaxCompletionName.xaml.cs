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
using TaxCalculator.Data.Model;
using TaxCalculator.Common.Mediator;
using TaxCalculator.Common;

namespace TaxCalculator.Controls.Popups
{
    /// <summary>
    /// Interaction logic for ChooseTaxCompletionName.xaml
    /// </summary>
    public partial class ChooseTaxCompletionName : Window, IDisposable
    {
        private bool closedFromUserAction = false;
        public ChooseTaxCompletionName()
        {
            InitializeComponent();
            txtName.Focus();
            Mediator.Instance.Register(MediatorActionType.SetSaveAsCallBackAction, SetSaveAsCallBackAction);
            Mediator.Instance.Register(MediatorActionType.NameChangeSetDefaultName, NameChangeSetDefaultName);
            this.Closing += new System.ComponentModel.CancelEventHandler(ChooseTaxCompletionName_Closing);
        }

        void ChooseTaxCompletionName_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closedFromUserAction)
            {
                saveAsCallBackAction(null);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            closedFromUserAction = true;
            if (string.IsNullOrEmpty(txtName.Text))
            {
                WindowHelper.OpenErrorDialog("Va rugam completati numele!");
                return;
            }
            saveAsCallBackAction(txtName.Text);
            CloseWindow();
        }

        Action<string> saveAsCallBackAction;
        public void SetSaveAsCallBackAction(object param)
        {
            saveAsCallBackAction = param as Action<string>;
        }

        public void NameChangeSetDefaultName(object param)
        {
            txtName.Text = param != null ? param.ToString() : "";
            txtName.CaretIndex = int.MaxValue;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            closedFromUserAction = true;
            saveAsCallBackAction(null);
            CloseWindow();
        }

        private void CloseWindow()
        {
            this.Close();
        }
        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetSaveAsCallBackAction, SetSaveAsCallBackAction);
            Mediator.Instance.Unregister(MediatorActionType.NameChangeSetDefaultName, NameChangeSetDefaultName);

        }
    }
}
