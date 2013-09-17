using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel.ViewModels.Model
{

    public enum FormulaType
    {
        Invalid,
        Condition,
        Value,
        SingleValue,
        ConstantValue
    }
    public class FormulaParam
    {
        public ParamType ParamType { get; set; }
        public object ParamData { get; set; }
        public override string ToString()
        {
            return ParamType + " " + ParamData.ToString();
        }
    }

    public enum ParamType
    {
        Sign,
        RowData,
        Formula,
        Value,
        ConditionData
    }

    public enum ParamSignType
    {
        Minus,
        Plus,
        Multiplication,
        Division

    }
    public class ParamSign
    {
        public ParamSignType Sign { get; set; }
        public override string ToString()
        {
            return Sign.ToString();
        }
    }
    public class RowData
    {
        public int NrCrt { get; set; }
        public override string ToString()
        {
            return NrCrt.ToString();
        }
    }
    public class ValueData
    {
        public decimal Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

}
