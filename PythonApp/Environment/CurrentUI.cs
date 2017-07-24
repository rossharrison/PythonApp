using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using ExcelDna.Integration;
using Excel = Microsoft.Office.Interop.Excel;

namespace PythonApp.Environment
{
    public class CurrentUI
    {
        public dynamic dnaapp = ExcelDnaUtil.Application;
        public Ast structure;
        public CurrentUI()
        {
            Excel.Application excelapp = (Excel.Application)dnaapp;
            structure = getChildren(excelapp);
        }


        public Ast getChildren(Excel.Application target, bool recursive = true)
        {
            Ast application = new Ast("application", target.Name);
            Ast workbooks = new Ast("workbooks", target.Workbooks.Count);

            foreach(Excel.Workbook wbk in target.Workbooks)
            {
                workbooks.AddChild(getChildren(wbk));
            }
            application.AddChild(workbooks);
            return application;
        }


        public Ast getChildren(Excel.Workbook target, bool recursive = true)
        {
            Ast workbook = new Ast("workbook", target.Name);
            Ast worksheets = new Ast("worksheets", target.Worksheets.Count);

            foreach (Excel.Worksheet wks in target.Worksheets)
            {
                worksheets.AddChild(getChildren(wks));
            }

            workbook.AddChild(worksheets);
            return workbook;
        }

        public Ast getChildren(Excel.Worksheet target, bool recursive = true)
        {
            Ast worksheet = new Ast("worksheet", target.Name);
            return worksheet;
        }



    }
}
