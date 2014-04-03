using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    class IndicatorVisibilityModel
    {
        public long IndicatorId { get; set; }
        public bool Hidden { get; set; }
    }

    internal class IndicatorRelationships
    {
        public List<List<long>> IndicatorStructureRelationships { get; set; }
    }
}
