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
            try
            {
                this.FormulaType = Model.FormulaType.Invalid;
                ExtractFormula(formula);
            }
            catch (Exception ex)
            {

                throw;
            }
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
            if (this.FormulaType == Model.FormulaType.Invalid)
            {
                throw new Exception("invalid formula type");
            }
            else if (this.FormulaType == Model.FormulaType.Value)
            {
                currentValue = ParseValue(taxIndicatorList, currentValue);
            }
            else if (this.FormulaType == Model.FormulaType.SingleValue)
            {
                currentValue = ParseSingleValue(taxIndicatorList, currentValue);
            }
            else if (this.FormulaType == Model.FormulaType.ConstantValue)
            {
                currentValue = ParseConstantValue(currentValue);
            }
            else if (this.FormulaType == Model.FormulaType.Condition)
            {
                currentValue = ParseCondition(taxIndicatorList, currentValue);
            }
            else
            {
                throw new Exception("unknown formula type");
            }
            return currentValue;
        }

        private decimal ParseValue(List<TaxIndicatorViewModel> taxIndicatorList, decimal currentValue)
        {
            if (Params.Count > 0)
            {
                int startIndex = 0;
                var firstParam = Params[0];
                if (firstParam.ParamType == ParamType.RowData)
                {
                    var value = GetById((firstParam.ParamData as RowData).NrCrt, taxIndicatorList).ValueFieldNumeric;
                    currentValue = value;
                    startIndex = 1;
                }
                for (int i = startIndex; i < Params.Count; i += 2)
                {
                    //get sign
                    var curParam = Params[i];
                    var curSign = (curParam.ParamData as ParamSign).Sign;
                    var currentSign = (curParam.ParamData as ParamSign).Sign == ParamSignType.Plus ? 1 : -1;
                    //get value
                    curParam = Params[i + 1];


                    if (curSign == ParamSignType.Plus)
                    {
                        var current = GetById((curParam.ParamData as RowData).NrCrt, taxIndicatorList);
                        currentValue += current.ValueFieldNumeric;
                    }
                    else if (curSign == ParamSignType.Minus)
                    {
                        var current = GetById((curParam.ParamData as RowData).NrCrt, taxIndicatorList);
                        currentValue -= current.ValueFieldNumeric;
                    }
                    else if (curSign == ParamSignType.Multiplication)
                    {
                        var current = (curParam.ParamData as ValueData).Value;
                        currentValue *= current;
                    }
                    else if (curSign == ParamSignType.Division)
                    {
                        var current = (curParam.ParamData as ValueData).Value;
                        currentValue /= current;
                    }
                    //currentValue += current.ValueFieldNumeric * currentSign;
                }
            }
            return currentValue;
        }

        private decimal ParseSingleValue(List<TaxIndicatorViewModel> taxIndicatorList, decimal currentValue)
        {
            if (Params.Count == 1)
            {
                var firstParam = Params[0];
                if (firstParam.ParamType == ParamType.RowData)
                {
                    var value = GetById((firstParam.ParamData as RowData).NrCrt, taxIndicatorList).ValueFieldNumeric;
                    currentValue = value;
                }
            }
            return currentValue;
        }

        private decimal ParseConstantValue(decimal currentValue)
        {
            if (Params.Count == 1)
            {
                var firstParam = Params[0];
                if (firstParam.ParamType == ParamType.Value)
                {
                    var value = Convert.ToInt32(firstParam.ParamData);
                    currentValue = value;
                }
            }
            return currentValue;
        }

        private decimal ParseCondition(List<TaxIndicatorViewModel> taxIndicatorList, decimal currentValue)
        {
            if (Params.Count == 1)
            {
                var firstParam = Params[0];
                if (firstParam.ParamType == ParamType.ConditionData)
                {
                    ConditionData cd = firstParam.ParamData as ConditionData;
                    bool ifResult = cd.IfStatement.ExecuteCondition(taxIndicatorList);
                    if (ifResult)
                    {
                        currentValue = cd.ThenStatement.Execute(taxIndicatorList);
                    }
                    else
                    {
                        currentValue = cd.ElseStatement.Execute(taxIndicatorList);
                    }

                }
            }
            return currentValue;
        }
        private TaxIndicatorViewModel GetById(int id, List<TaxIndicatorViewModel> taxIndicatorList)
        {
            var existing = taxIndicatorList.FirstOrDefault(p => p.NrCrt == id);
            if (existing == null)
            {
                throw new Exception("could not find taxIndicator with id " + id);
            }
            return existing;
        }

        /// <summary>
        /// from rd.(1-2-3-4+5+6-7+8-9)-Rd.(10-11+12)
        /// to  rd.(1-2-3-4+5+6-7+8-9-10+11-12)
        /// </summary>
        /// <param name="formula"></param>
        private void NormalizeFormula(ref string formula)
        {
            try
            {
                var formulaParts = formula.Split(new string[] { "rd." }, StringSplitOptions.RemoveEmptyEntries);
                if (formulaParts.Length > 1)
                {
                    for (int j = 0; j < formulaParts.Length; j++)
                    {
                        if (!formulaParts[j].StartsWith("("))
                        {
                            formulaParts[j] = "(" + formulaParts[j] + ")";
                        }
                    }
                    //formulaParts[0].Remove(formulaParts[0].Length - 1, 1);
                    string resultedFormula = formulaParts[0].Remove(formulaParts[0].Length - 2, 2);
                    //string resultedFormula = formulaParts[0].Remove(formulaParts[0].Length - 1, 1);
                    //for each part starting from the second one, apply the sign (if minus in front of the rd)
                    for (int i = 1; i < formulaParts.Length; i++)
                    {
                        var previous = formulaParts[i - 1];
                        var current = formulaParts[i].ToList();
                        var lastSign = previous[previous.Length - 1] == '-' ? -1 : 1;

                        if (current[0] == '(' && current[current.Count - 1] == ')')
                        {
                            current.RemoveAt(0);
                            current.RemoveAt(current.Count - 1);
                        }
                        if (current[0] != '-' && current[0] != '+')
                        {
                            current.Insert(0, '+');
                        }
                        for (int j = 0; j < current.Count; j++)
                        {
                            if (current[j] == '-')
                            {
                                current[j] = '+';
                            }
                            else if (current[j] == '+')
                            {
                                current[j] = '-';
                            }
                        }
                        string resultedString = "";
                        current.ForEach(p => resultedString += p.ToString());
                        resultedFormula += resultedString;

                    }
                    resultedFormula += ")";
                    formula = "rd." + resultedFormula;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private void ExtractFormula(string formula)
        {
            formula = formula.Trim().ToLower().Replace(" ", string.Empty);
            NormalizeFormula(ref formula);
            int constValue = 0;
            if (int.TryParse(formula, out constValue))
            {
                //if the formula is a constant value
                this.FormulaType = Model.FormulaType.ConstantValue;
                this.Params.Add(new FormulaParam()
                {
                    ParamType = ParamType.Value,
                    ParamData = constValue
                });
            }
            if (formula.StartsWith("rd."))
            {
                ParseValueFormula(formula);
            }
            else if (formula.StartsWith("if"))
            {
                formula = BuildIfStatement(formula);
            }
            else
            {
                throw new Exception("invalid formula");
            }
        }

        private void ParseValueFormula(string formula)
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
                    onlyFormula = RemoveParanthesis(onlyFormula);
                    try
                    {
                        this.FormulaType = Model.FormulaType.Value;
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
                        throw;
                    }

                }
                else
                {
                    //number only
                    //rd.23
                    int onlyNumber = 0;
                    if (int.TryParse(onlyFormula, out onlyNumber))
                    {
                        this.FormulaType = Model.FormulaType.SingleValue;
                        FormulaParam par = new FormulaParam();
                        par.ParamType = ParamType.RowData;
                        par.ParamData = new RowData() { NrCrt = Math.Abs(onlyNumber) };
                        Params.Add(par);
                    }
                    else
                    {
                        //split by *
                        string[] formulaParts = onlyFormula.Split(new char[] { '*' });
                        if (formulaParts.Length == 2)
                        {
                            //get the first number
                            if (int.TryParse(formulaParts[0], out onlyNumber))
                            {
                                this.FormulaType = Model.FormulaType.Value;
                                FormulaParam par = new FormulaParam();
                                par.ParamType = ParamType.RowData;
                                par.ParamData = new RowData() { NrCrt = Math.Abs(onlyNumber) };
                                Params.Add(par);

                                par = new FormulaParam();
                                par.ParamType = ParamType.Sign;
                                par.ParamData = new ParamSign() { Sign = ParamSignType.Multiplication };
                                Params.Add(par);

                                //get the second component
                                var secondPart = formulaParts[1];
                                //test if percentage
                                if (secondPart.EndsWith("%"))
                                {
                                    //rd.41*16%
                                    secondPart = secondPart.TrimEnd('%');
                                    int percentage = 0;
                                    if (int.TryParse(secondPart, out percentage))
                                    {
                                        par = new FormulaParam();
                                        par.ParamType = ParamType.Value;
                                        par.ParamData = new ValueData() { Value = (decimal)percentage / 100 };
                                        Params.Add(par);
                                    }
                                    else
                                    {
                                        throw new Exception("cannot parse");
                                    }
                                }
                                else
                                {
                                    //test for "/" component
                                    //rd.45*3/1000
                                    if (secondPart.Contains('/'))
                                    {
                                        var divisionNumbers = secondPart.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (divisionNumbers.Length == 2)
                                        {
                                            int first = 0;
                                            int second = 0;
                                            if (int.TryParse(divisionNumbers[0], out first) && int.TryParse(divisionNumbers[1], out second))
                                            {
                                                this.FormulaType = Model.FormulaType.Value;
                                                par = new FormulaParam();
                                                par.ParamType = ParamType.Value;
                                                par.ParamData = new ValueData() { Value = (decimal)first / second };
                                                Params.Add(par);
                                            }
                                            else
                                            {
                                                throw new Exception("parse error");
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(" error");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("parse error");

                            }
                        }
                        // throw new Exception("invalid formula: " + formula);
                    }
                }
            }
            //else
            //{
            //}
        }

        private string BuildIfStatement(string formula)
        {
            //IF(C32*2%>0;C32*2%;0)
            //IF(C31-C33>0;C31-C33;0)
            //IF(C37*2%>0;C37*2%;0)
            //IF(C36-C38>0;C36-C38;0)
            //IF(C15>0;0;-C15)
            //IF((C46>C48);C46;C48)
            //IF(C49<C50;C49;C50)

            //if condition then value else value.
            //obiect nou ce tine acest if si se evalueaza pe Evaluate
            this.FormulaType = Model.FormulaType.Condition;

            //remove the if
            formula = formula.Remove(0, 2);
            formula = formula.Replace("c", "rd.");

            if (formula.StartsWith("(") && formula.EndsWith(")"))
            {
                //formula
                //IF(C31-C33>0;C31-C33;0)
                //remove ()
                formula = RemoveParanthesis(formula);
            }
            //C31-C33>0;C31-C33;0
            //split and get the if members
            string[] ifMembers = formula.Split(new char[] { ';' });
            if (ifMembers.Length != 3)
            {
                throw new Exception();
            }
            //get the condition
            ifMembers[0] = RemoveParanthesis(ifMembers[0]);
            IfStatement ifStatement = new IfStatement(ifMembers[0]);

            ThenStatement ts = new ThenStatement(ifMembers[1]);
            ElseStatement es = new ElseStatement(ifMembers[2]);

            this.Params.Add(new FormulaParam()
            {
                ParamType = ParamType.ConditionData,
                ParamData = new ConditionData()
                {
                    IfStatement = ifStatement,
                    ThenStatement = ts,
                    ElseStatement = es
                }
            });
            return formula;
        }

        private static string RemoveParanthesis(string onlyFormula)
        {
            if (onlyFormula.StartsWith("(") && onlyFormula.EndsWith(")"))
            {
                onlyFormula = onlyFormula.Remove(0, 1);
                onlyFormula = onlyFormula.Remove(onlyFormula.Length - 1, 1);
            }
            return onlyFormula;
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
            if (Constants.AllowedRdOperations.Contains(firstChar))
            {
                if (firstChar == '-')
                {
                    sign = -1;
                }
                formula = formula.Remove(0, 1);
            }
            int nextNumber = 0;
            var parts = formula.Split(Constants.AllowedRdOperations);
            if (parts.Length > 0)
            {
                nextNumber = Convert.ToInt32(parts[0]);
                formula = formula.Remove(0, parts[0].Length);

                return nextNumber * sign;
            }
            else
            {
                return 0;
                throw new Exception("invalid input: " + formula);
            }
        }
    }
}
