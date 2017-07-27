using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using Microsoft.Office.Interop;
using python;
using Data;
namespace PythonApp.UI
{
    
    public static class ExcelFormulas
    {
        [ExcelFunction(Description = "Run Python Script")]
        public static string PythonScript(string input)
        {
            PythonResponse py = new execute(input).py;

            if (py.StdOut=="")
            {
                return py.ExceptionMessage;
            }
            else
            { 
                return py.StdOut.ToString();
            }
        }

    }
}
