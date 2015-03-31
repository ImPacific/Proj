using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel; //use the reference in your code
using System.Reflection;


namespace PCComm
{
    class excel_helper
    {
        //private static int DECLINATION_COLNUM = 1;
        //private static int CB4_COLNUM = 3;
        //private static int CB7_COLNUM = 4;
        //private static int CB11_COLNUM = 5;

        //private static int CB4_DISTANCE_COLNUM = 6;
        //private static int CB7_DISTANCE_COLNUM = 7;
        //private static int CB11_DISTANCE_COLNUM = 8;

        //private static int CB4_DURATION_COLNUM = 9;
        //private static int CB7_DURATION_COLNUM = 10;
        //private static int CB11_DURATION_COLNUM = 11;

        //Add this codes in your progam code
        private static Microsoft.Office.Interop.Excel.ApplicationClass appExcel;
        private static Workbook newWorkbook = null;
        private static _Worksheet objsheet = null;

        //Method to initialize opening Excel
        public static void excel_init(String path)
        {
            appExcel = new Microsoft.Office.Interop.Excel.ApplicationClass();

            if (System.IO.File.Exists(path))
            {
                // then go and load this into excel
                newWorkbook = appExcel.Workbooks.Open(path, true, true);
                objsheet = (_Worksheet)appExcel.ActiveWorkbook.ActiveSheet;
            }
            else
            {
                MessageBox.Show("Unable to open file!");
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appExcel);
                appExcel = null;
                System.Windows.Forms.Application.Exit();
            }

        }

        //Method to get value; cellname is A1,A2, or B1,B2 etc...in excel.
        public static string excel_getValue(string cellname)
        {
            string value = string.Empty;
            try
            {
                value = objsheet.get_Range(cellname).get_Value().ToString();
            }
            catch
            {
                value = "";
            }

            return value;
        }

        //Method to close excel connection
        public static void excel_close()
        {
            if (appExcel != null)
            {
                try
                {
                    newWorkbook.Close();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(appExcel);
                    appExcel = null;
                    objsheet = null;
                }
                catch (Exception ex)
                {
                    appExcel = null;
                    MessageBox.Show("Unable to release the Object " + ex.ToString());
                }
                finally
                {
                    GC.Collect();
                }
            }
        }
        private static bool _Validate_Date(string _strDate)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Range currentFind = null;
                currentFind = objsheet.Cells.Find(_strDate, Type.Missing,
                Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues, Microsoft.Office.Interop.Excel.XlLookAt.xlWhole,
                Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows, Microsoft.Office.Interop.Excel.XlSearchDirection.xlNext, false,
                Type.Missing, Type.Missing);
                if (currentFind == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool Validate_Date(string strDate)
        {
            return _Validate_Date(strDate);
        }

        public static string Find_ActValue_Frm_Date(string strDate, int actuatorval)
        {
            object False = false;
            object True = true;

            Microsoft.Office.Interop.Excel.Range currentFind = null;

            currentFind = objsheet.Cells.Find(strDate, Type.Missing,
            Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues, Microsoft.Office.Interop.Excel.XlLookAt.xlWhole,
            Microsoft.Office.Interop.Excel.XlSearchOrder.xlByRows, Microsoft.Office.Interop.Excel.XlSearchDirection.xlNext, false,
            Type.Missing, Type.Missing);

        //    MessageBox.Show(currentFind.get_Address(true, true, XlReferenceStyle.xlA1, false, Missing.Value));

            int row = currentFind.Row;
            int col = currentFind.Column;
            string cols = col.ToString();
            string rows = row.ToString();

            return(rows);
           // return excel_getValue(GetColumnName(col) + row.ToString());
        }
        public static string Get_Actuator_Value(string strDate, int actuatorval)
        {
            return Find_ActValue_Frm_Date(strDate, actuatorval);
        }

        private static string GetColumnName(int columnIndex)
        {
            if (columnIndex < 0) throw new ArgumentOutOfRangeException("columnIndex", "Column index cannot be negative.");

            var dividend = columnIndex + 1;
            var columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }
    }
}
