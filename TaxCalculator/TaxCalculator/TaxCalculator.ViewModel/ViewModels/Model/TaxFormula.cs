using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Common;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    /// <summary>
    /// //rd.(1-2-3-4+5+6-7+8-9)-Rd.(10-11+12)
    //rd.29
    //rd.47*20/100
    //IF((C46>C48);C46;C48)
    //rd.41*16%
    /// </summary>
    public class TaxFormula
    {
        public TaxFormula(string formula)
            : this()
        {
            ExtractFormula(formula);
        }

        public TaxFormula()
        {
            Params = new List<FormulaParam>();
        }

        public List<FormulaParam> Params { get; set; }
        public FormulaType FormulaType { get; set; }

        public decimal Execute(List<TaxIndicatorViewModel> taxIndicatorList)
        {
            decimal currentValue = 0;
            if (Params.Count > 0)
            {
                var firstParam = Params[0];
                if (firstParam.ParamType == ParamType.RowData)
                {
                    var value = GetById((firstParam.ParamData as RowData).NrCrt, taxIndicatorList).ValueFieldNumeric;
                    currentValue = value;
                }
                for (int i = 0; i < Params.Count; i += 2)
                {
                    //get sign
                    var curParam = Params[i];
                    var currentSign = (curParam.ParamData as ParamSign).Sign == ParamSignType.Plus ? 1 : -1;
                    //get value
                    curParam = Params[i + 1];
                    var current = GetById((curParam.ParamData as RowData).NrCrt, taxIndicatorList);

                    currentValue += current.ValueFieldNumeric * currentSign;
                }
            }
            return currentValue;
        }
        private TaxIndicatorViewModel GetById(int id, List<TaxIndicatorViewModel> taxIndicatorList)
        {
            return taxIndicatorList.First(p => p.NrCrt == id);
        }
        private void ExtractFormula(string formula)
        {
            formula = formula.Trim();
            if (formula.StartsWith("rd."))
            {
                //string[] formulas = formula.Split(new string[] { "rd." });
                // if (formulas.Length == 1)
                {
                    string onlyFormula = formula.Replace("rd.", "");
                    if (onlyFormula.StartsWith("(") && onlyFormula.EndsWith(")"))
                    {
                        //formula
                        //rd.(1-2-3-4+5+6-7+8-9)
                        //remove ()
                        onlyFormula = onlyFormula.Remove(0, 1);
                        onlyFormula = onlyFormula.Remove(onlyFormula.Length - 1, 1);

                        try
                        {
                            Params = new List<FormulaParam>();
                            var nextNr = GetNextNumberWithSign(ref onlyFormula);
                            while (nextNr != 0)
                            {

                                FormulaParam par = new FormulaParam();
                                par.ParamType = ParamType.Sign;
                                par.ParamData = new ParamSign() { Sign = nextNr > 0 ? ParamSignType.Plus : ParamSignType.Minus };
                                Params.Add(par);

                                par = new FormulaParam();
                                par.ParamType = ParamType.RowData;
                                par.ParamData = new RowData() { NrCrt = Math.Abs(nextNr) };
                                Params.Add(par);
                                nextNr = GetNextNumberWithSign(ref onlyFormula);
                            }
                        }
                        catch
                        {
                        }
                        //split by operators
                        //var parts = onlyFormula.Split(TaxCalculatorConstants.AllowedRdOperations);
                        //if (parts.Length > 0)
                        //{
                        //    foreach (var part in parts)
                        //    {



                        //    }

                        //}
                        //else
                        //{
                        //    throw new Exception();
                        //}
                    }
                    else
                    {
                        //number only
                        //rd.23

                    }
                }
                //else
                //{
                //}
            }
        }

        private int GetNextNumberWithSign(ref string formula)
        {
            formula = formula;
            if (string.IsNullOrWhiteSpace(formula))
            {
                return 0;
            }
            var firstChar = formula[0];
            sbyte sign = 1;
            if (TaxCalculatorConstants.AllowedRdOperations.Contains(firstChar))
            {
                if (firstChar == '-')
                {
                    sign = -1;
                }
                formula = formula.Remove(0, 1);
            }
            int nextNumber = 0;
            var parts = formula.Split(TaxCalculatorConstants.AllowedRdOperations);
            if (parts.Length > 0)
            {
                nextNumber = Convert.ToInt32(parts[0]);
                formula = formula.Remove(0, parts[0].Length);

                return nextNumber * sign;
            }
            else return 0;
            throw new Exception("invalid input: " + formula);
        }
    }

    public enum FormulaType
    {
        Condition,
        Value
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
        Formula
    }

    public enum ParamSignType
    {
        Minus,
        Plus

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
}
