using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common.Mediator;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class ReportsVm : BaseViewModel, IDisposable
    {
        #region Fields

        #endregion

        #region Constructor

        public ReportsVm()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetReportData, SetReportData);
        }

        private void InitializeCommands()
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods
        
        public void SetReportData(object param)
        {

        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            base.Dispose();
        }

     
        #endregion

    }
}
