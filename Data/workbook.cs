using Excel = Microsoft.Office.Interop.Excel;
using ExcelDna.Integration;
using System.Resources;


namespace Data
{
    public static class workbook
    {
        public static ObjectHandler application;
        public static dynamic dnaapp;

        static workbook()
        {
            dnaapp = (dynamic)ExcelDnaUtil.Application;
            application = new ObjectHandler("Tracker", "Application", ExcelDnaUtil.Application);
            application.environment.AddChild(new Ast("dnaapp", dnaapp));
            application.environment.AddChild(new Ast("excelapp", (Excel.Application)dnaapp));
        }

        public static Excel.Application excelapp()
        {
            return (Excel.Application)application.environment.getFirstChild("excelapp")?.value;
        }

        public static Ast metadata()
        {
            return application.environment.getFirstChild("metadata");
        }

        public static DataFrame metadata(string name)
        {
            Ast meta = metadata();
            Ast metadataItem = meta.getFirstChild(name);
            DataFrame DataFrame = (DataFrame)metadataItem.value;
            return DataFrame;
        }

        public static void Display(string message)
        {
            Excel.Application excelapp = (Excel.Application)application.environment.getFirstChild("excelapp").value;
            excelapp.StatusBar = message;
        }

        public static object getResource(string name)
        {
            ResourceManager rm = Data.Properties.Resources.ResourceManager;
            return rm.GetObject(name);
        }

        public static string getString(string name)
        {
            ResourceManager rm = Data.Properties.Resources.ResourceManager;
            return rm.GetString(name);

        }
    }

    public static class API
    {
        public static string url;
        public static string datareader;

        static API()
        {
            url = workbook.getString("DebugAPI");
            datareader = workbook.getString("ReaderAPI");
        }

    }
}