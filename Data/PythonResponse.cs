using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class PythonResponse
    {
        public string ScriptPath { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Arg3 { get; set; }
        public string Arg4 { get; set; }
        public string Arg5 { get; set; }
        public string ScriptName { get; set; }
        public string Result { get; set; }
        public int ExitCode { get; set; }
        public string OtherCode { get; set; }
        public string ExceptionMessage { get; set; }
        public string StdOut { get; set; }

    }
}
