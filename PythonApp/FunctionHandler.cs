using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using python;

namespace PythonApp
{
    public static class FunctionHandler
    {
        [ExcelFunction(Description = "Runs a Python Function",
                Category = "Python")]
        public static string PythonFunction(string value)
        {
            return "initial test works";
        }


        [ExcelFunction(Description = "Runs a Python Function",
                Category = "Python")]
        public static string PythonScript(string value)
        {
            var a = new execute(value);
            if(a.ExceptionMessage==null)
            {
                return a.StdOut.ToString();
            }
            else
            {
            return "Error: " + a.ExceptionMessage;
            }
        }

    }
}
