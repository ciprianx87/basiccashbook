using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class IfStatement
    {
        private string ifCondition;

        public IfStatement()
        {

        }

        public IfStatement(string ifCondition)
        {

            this.ifCondition = ifCondition;
            ParseCondition();
        }

        //public IfStatement(List<TaxIndicatorViewModel> taxIndicatorList)
        //{
        //    this.taxIndicatorList = taxIndicatorList;
        //}
        //private List<TaxIndicatorViewModel> taxIndicatorList;
        public TaxFormula Left { get; set; }
        public TaxFormula Right { get; set; }
        public string Operator { get; set; }

        private void ParseCondition()
        {

            //C31-C33>0
            string op = "";
            string leftStr = "";
            string rightStr = "";


            //extract the condition parts
            GetOperatorAndComparisonMembers(ifCondition, ref op, ref leftStr, ref rightStr);

            Operator = op;

            Left = new TaxFormula(leftStr);
            Right = new TaxFormula(rightStr);

        }

        private void GetOperatorAndComparisonMembers(string condition, ref string op, ref string left, ref string right)
        {
            string[] parts = null;
            if (condition.Contains('<'))
            {

                parts = condition.Split(new char[] { '<' });
                if (parts.Length == 2)
                {
                    op = "<";
                }
                else
                {
                    throw new Exception("invalid condition: " + condition);
                }
            }
            else if (condition.Contains('>'))
            {
                parts = condition.Split(new char[] { '>' });
                if (parts.Length == 2)
                {
                    op = ">";
                }
                else
                {
                    throw new Exception("invalid condition: " + condition);
                }
            }
            else
            {
                throw new Exception("invalid condition: " + condition);
            }
            left = parts[0];
            right = parts[1];

        }

        public bool ExecuteCondition(List<TaxIndicatorViewModel> taxIndicatorList)
        {
            var left = Left.Execute(taxIndicatorList);
            var right = Right.Execute(taxIndicatorList);
            bool result = false;
            switch (Operator)
            {
                case "<":
                    result = left < right;
                    break;
                case ">":
                    result = left > right;
                    break;
                default:
                    throw new Exception("invalid operator: " + Operator);
            }
            return result;
        }
    }
    public class ConditionData
    {
        public IfStatement IfStatement { get; set; }
        public ThenStatement ThenStatement { get; set; }
        public ElseStatement ElseStatement { get; set; }
    }
}
