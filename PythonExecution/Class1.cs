using System;
using System.IO;
using System.Text;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace python
{
    public class execute
    {
        public execute(string str)
        {
            ScriptEngine scriptEngine = Python.CreateEngine();
            ScriptSource scriptSource = scriptEngine
                .CreateScriptSourceFromString(str);
              //.CreateScriptSourceFromFile(ScriptPath
                //                          , Encoding.ASCII
                  //                        , SourceCodeKind.File);

            ScriptScope sys = scriptEngine.GetSysModule();
            var argv = new List { ScriptName, Arg1, Arg2, Arg3, Arg4, Arg5 };
            sys.SetVariable("argv", argv);

            using (var memoryStream = new MemoryStream())
            {
                scriptEngine.Runtime.IO.SetOutput(memoryStream, new StreamWriter(memoryStream));
                try
                {
                    object thing = scriptSource.Execute();
                    if (thing != null)
                        Result = thing.ToString();
                }
                catch (SystemExitException e)
                {
                    object otherCode;
                    ExitCode = e.GetExitCode(out otherCode);
                    if (otherCode != null)
                        OtherCode = otherCode.ToString();
                }
                catch (Exception e)
                {
                    var eo = scriptEngine.GetService<ExceptionOperations>();
                    ExceptionMessage = eo.FormatException(e);
                }
                finally
                {
                    var length = (int)memoryStream.Length;
                    var bytes = new byte[length];
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.Read(bytes, 0, length);
                    StdOut = Encoding.UTF8.GetString(bytes, 0, length).Trim();
                }
            }
        }
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
