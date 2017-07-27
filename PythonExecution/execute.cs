using System;
using System.IO;
using System.Text;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using Python.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using Data;

namespace python
{
    public class execute
    {
        public PythonResponse py = new PythonResponse();

        public execute(string str)
        {
            
            ScriptEngine scriptEngine = IronPython.Hosting.Python.CreateEngine();

            scriptEngine.ImportModule("PythonDateTime");
            ICollection<string> paths = scriptEngine.GetSearchPaths();

            if(!paths.Contains(@"\\pol-ad-d01731\site\scripts"))
            { 
                paths.Add(@"\\pol-ad-d01731\site\scripts");
                scriptEngine.SetSearchPaths(paths);
            }

            ScriptSource scriptSource = scriptEngine
                .CreateScriptSourceFromString(str + "\n");
            //.CreateScriptSourceFromFile(ScriptPath
            //                          , Encoding.ASCII
            //                        , SourceCodeKind.File);

            ScriptScope sys = scriptEngine.GetSysModule();
            var argv = new List { py.ScriptName, py.Arg1, py.Arg2, py.Arg3, py.Arg4, py.Arg5 };
            sys.SetVariable("argv", argv);

            using (var memoryStream = new MemoryStream())
            {
                scriptEngine.Runtime.IO.SetOutput(memoryStream, new StreamWriter(memoryStream));
                try
                {
                    object thing = scriptSource.Execute();
                    if (thing != null)
                        py.Result = thing.ToString();
                }
                catch (SystemExitException e)
                {
                    object otherCode;
                    py.ExitCode = e.GetExitCode(out otherCode);
                    if (otherCode != null)
                        py.OtherCode = otherCode.ToString();
                }
                catch (Exception e)
                {
                    var eo = scriptEngine.GetService<ExceptionOperations>();
                    py.ExceptionMessage = eo.FormatException(e);
                }
                finally
                {
                    var length = (int)memoryStream.Length;
                    var bytes = new byte[length];
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.Read(bytes, 0, length);
                    py.StdOut = Encoding.UTF8.GetString(bytes, 0, length).Trim();
                }
            }
        }




        //public string ScriptPath { get; set; }
        //public string Arg1 { get; set; }
        //public string Arg2 { get; set; }
        //public string Arg3 { get; set; }
        //public string Arg4 { get; set; }
        //public string Arg5 { get; set; }
        //public string ScriptName { get; set; }
        //public string Result { get; set; }
        //public int ExitCode { get; set; }
        //public string OtherCode { get; set; }
        //public string ExceptionMessage { get; set; }
        //public string StdOut { get; set; }
    }
}
