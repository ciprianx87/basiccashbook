using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.ViewModels.Model;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Globalization;

namespace TaxCalculator.ViewModel.Helper
{
    public class ExcelExport
    {
        public static void ExportToExcel(PrintDataModel printModel, string fileName)
        {
            var OriginalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            // Load Excel application
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            // Create empty workbook
            excel.Workbooks.Add();

            // Create Worksheet from active sheet
            Microsoft.Office.Interop.Excel._Worksheet workSheet = excel.ActiveSheet;

            try
            {
                var firstPage = printModel.Pages[0];
                bool isSecondTypeReport = firstPage.Version2Visibility == System.Windows.Visibility.Visible && firstPage.FirstPageData.SecondTypeReport;
                // ------------------------------------------------
                // Creation of header cells
                // ------------------------------------------------
                workSheet.Cells[1, "B"] = "Societatea: " + firstPage.FirstPageData.Company;
                workSheet.Cells[2, "B"] = "Adresa: " + firstPage.FirstPageData.Address;
                workSheet.Cells[3, "B"] = "Cui: " + firstPage.FirstPageData.Cui;

                workSheet.Cells[5, "B"] = "CALCUL IMPOZIT PROFIT";
                SetHeaderTitle(workSheet, 5, "B");
                AlignToCenter(workSheet, 5, "B");

                workSheet.Cells[5, "C"] = "Perioada";
                workSheet.Cells[6, "C"] = firstPage.FirstPageData.MonthYear;
                SetBold(workSheet, 5, "C");
                SetBold(workSheet, 6, "C");
                AlignToCenter(workSheet, 5, "C");
                AlignToCenter(workSheet, 6, "C");

                int row = 7;
                if (isSecondTypeReport)
                {
                    row++;
                    workSheet.Cells[4, "C"] = "Rectificativa";
                    workSheet.Cells[5, "D"] = "Perioada";
                    workSheet.Cells[6, "D"] = firstPage.FirstPageData.MonthYear;
                    SetBold(workSheet, 5, "D");
                    SetBold(workSheet, 6, "D");

                    AlignToCenter(workSheet, 4, "C");
                    AlignToCenter(workSheet, 5, "D");
                    AlignToCenter(workSheet, 6, "D");

                    workSheet.Cells[7, "C"] = "inainte de impozit";
                    workSheet.Cells[7, "D"] = "dupa impozit";
                    AlignToCenter(workSheet, 7, "C");
                    AlignToCenter(workSheet, 7, "D");

                }
                var allPageData = new List<PrintRow>();
                foreach (var page in printModel.Pages)
                {
                    allPageData.AddRange(page.Rows);
                }
               // for (int i = 0; i < gvExportExcel.Rows.Count; i++) 

                //remove the title--replace it with something else
                allPageData.RemoveAt(0);
                foreach (var page in allPageData)
                {
                    workSheet.Cells[row, "A"] = page.NrCrt;
                    workSheet.Cells[row, "B"] = page.Description;
                    workSheet.Cells[row, "C"] = GetExcelStyleValue(page.Value);
                    //set column format to text
                    //workSheet.Range["C" + row].NumberFormat = "@";               

                    if (page.Type == Data.Model.TaxIndicatorType.Calculat)
                    {
                        var range = workSheet.Range["B" + row];
                        range.Font.Bold = true;
                        AlignToRight(workSheet, row, "C");
                    }
                    if (page.Type == Data.Model.TaxIndicatorType.Text)
                    {
                        string columnName = "B";
                        SetTextStyle(workSheet, row, "A");
                        SetTextStyle(workSheet, row, "B");
                        SetTextStyle(workSheet, row, "C");
                        AlignToCenter(workSheet, row, "B");
                    }
                    if (page.Type == Data.Model.TaxIndicatorType.Numeric)
                    {
                        AlignToRight(workSheet, row, "C");
                    }
                    if (isSecondTypeReport)
                    {
                        //2nd type report
                        AlignToRight(workSheet, row, "D");
                        workSheet.Cells[row, "D"] = GetExcelStyleValue(page.InitialValue);
                    }
                    row++;
                }             

                Range er = workSheet.get_Range("A:A", System.Type.Missing);
                er.EntireColumn.ColumnWidth = 10;
                er = workSheet.get_Range("B:B", System.Type.Missing);
                er.EntireColumn.ColumnWidth = 80;
                er = workSheet.get_Range("C:C", System.Type.Missing);
                er.NumberFormat = "@";
                er.EntireColumn.ColumnWidth = 20;
                er = workSheet.get_Range("D:D", System.Type.Missing);
                er.EntireColumn.ColumnWidth = 20;

                er.NumberFormat = "@";
                //add the last rows
                row += 3;
                workSheet.Cells[row, "B"] = "INTOCMIT,";
                row++;
                workSheet.Cells[row, "B"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                workSheet.Cells[row, "B"] = printModel.Pages.Last().LastPageData.CreatedBy;
                workSheet.Cells[row, "B"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                row++;
                workSheet.Cells[row, "C"] = "VERIFICAT,";
                workSheet.Cells[row, "C"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                row++;
                workSheet.Cells[row, "C"] = printModel.Pages.Last().LastPageData.VerifiedBy;
                workSheet.Cells[row, "C"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                // Apply some predefined styles for data to look nicely :)
                //workSheet.Range["A1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);

                // Define filename
                // string fileName = string.Format(@"{0}\ExcelData.xls", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                // Save this data as a file
                workSheet.SaveAs(fileName);

                // Display SUCCESS message
                Console.WriteLine(string.Format("The file '{0}' is saved successfully!", fileName));
                //MessageBox.Show(string.Format("The file '{0}' is saved successfully!", fileName));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);

                //MessageBox.Show("Exception",
                //    "There was a PROBLEM saving Excel file!\n" + exception.Message,
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    // Quit Excel application
                    excel.Quit();

                    // Release COM objects (very important!)
                    if (excel != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                    if (workSheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                    // Empty variables
                    excel = null;
                    workSheet = null;

                    // Force garbage collector cleaning
                    GC.Collect();

                }
                catch { }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = OriginalCulture;
                }
            }
        }
        private static string GetExcelStyleValue(string value)
        {
            return "=\"" + value + "\"";
        }
        private static void AlignToCenter(Microsoft.Office.Interop.Excel._Worksheet workSheet, int rowNr, string column)
        {
            workSheet.Cells[rowNr, column].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
        }

        private static void AlignToRight(Microsoft.Office.Interop.Excel._Worksheet workSheet, int rowNr, string column)
        {
            workSheet.Cells[rowNr, column].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
        }

        private static void SetBold(Microsoft.Office.Interop.Excel._Worksheet workSheet, int row, string columnName)
        {
            var range = workSheet.Range[columnName + row];
            range.Font.Bold = true;
            range.Font.Name = "Times New Roman";
        }

        private static void SetHeaderTitle(Microsoft.Office.Interop.Excel._Worksheet workSheet, int row, string columnName)
        {
            var range = workSheet.Range[columnName + row];
            range.Font.Bold = true;
            range.Font.Name = "Times New Roman";
            range.Font.Underline = true;
            range.Font.Size = 14;
            var alignment = range.HorizontalAlignment;
        }

        private static void SetTextStyle(Microsoft.Office.Interop.Excel._Worksheet workSheet, int row, string columnName)
        {
            var range = workSheet.Range[columnName + row];
            range.Font.Bold = true;
            range.Font.Underline = true;
            range.Font.Name = "Times New Roman";

        }
    }
}
