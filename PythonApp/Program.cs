using PythonApp.Environment;
using ExcelDna.Integration;
using DockSample;



namespace PythonApp
{
    public class Program : IExcelAddIn
    {
        public static MainForm PythonIDE;
        public static CurrentUI structure;

        public void AutoOpen()
        {
            KeyboardHook.SetHook();
            structure =  new CurrentUI();
        }

        public void AutoClose()
        {
            KeyboardHook.ReleaseHook();
        }

    }
}
