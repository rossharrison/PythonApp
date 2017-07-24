using System;
using System.Collections.Generic;
using System.Linq;
using Data.Interface;
using Data.Encoding;
using System.Reflection;
using System.Text;

namespace Data
{
    public class DataFrame : IData
    {
        public Dictionary<string, DataFrame> metadata { get; set; } = new Dictionary<string, DataFrame>();
        public string Name { get; set; }
        public string name { get;set; }
        public List<Ast> query { get; set; }
        public string[,] Datatable { get; set; }
        public string[] RowId { get; set; }
        public string[] ColumnName { get; set; }
        public string[] ColumnFormat { get; set; }
        public int LastRow { get; set; }
        public int LastColumn { get; private set; }
        public int Ptr { get; set; }
        public string[] underlyingName { get; set; }
        public string type { get; set; } = "DataFrame";
        public string CRUDtype { get; set; }
        //public Encode encoder = new Encode();



        public DataFrame(Ast ResponseTree)
        {
            while(ResponseTree.Children.Count() == 1)
            {
                ResponseTree = (Ast)ResponseTree.Children[0];

            }

            name = (string)ResponseTree.GetValue("name");
            Ast columns = (Ast)ResponseTree.GetValue("Columns");
            Ast rows = (Ast)ResponseTree.GetValue("Rows");
            Ast data = (Ast)ResponseTree.GetValue("Data");

            Resize(rows.children.Count(), columns.children.Count());

            int i = 0;
            foreach (var child in columns.children)
            {
                ColumnName[i] = Encode.Base64Decode((string)child.value);
                i++;
            }

            int j = 0;
            foreach (var child in rows.children)
            {
                RowId[j] = Encode.Base64Decode((string)child.value);
                j++;
            }

            i = i + 1;
            Ptr = 0;

            foreach (var child in data.children)
            {
                string str = Encode.Base64Decode((string)child.value);
                str = str == "=" ? "" : str;
                Datatable[(Ptr / (LastColumn + 1)), Ptr % (LastColumn + 1)] = str;
                Ptr++;
            }
        }

        public DataFrame(List<object> listitems)
        {
            List<PropertyInfo> fields = listitems.GetType().GetProperties().ToList();
            Resize(fields.Count(), listitems.Count);

            int i = 0;
            int j = 0;
            object firstItem = listitems[0];

            foreach (var prop in fields)
            {
                ColumnName[j] = prop.Name;
                ColumnFormat[j] = prop.PropertyType.ToString();

                foreach (object item in listitems)
                {
                    string v = item.GetType().GetProperty(prop.Name)?.GetValue(item, null)?.ToString() ?? "";
                    SetValue(j, i, v);
                    i++;
                }
                i = 0;
                j++;
            }
        }

        public DataFrame()
        {
            query = new List<Ast>();
        }

        public DataFrame(int rows, int columns)
        {
            Resize(rows, columns);
        }

        internal DataFrame Filter(string v, int actionId)
        {
            throw new NotImplementedException();
        }



        public DataFrame GetRow(int i)
        {
            DataFrame grid = new DataFrame();

            grid.Resize(0, this.LastColumn);

            for (int c = 0; c <= LastColumn; c++)
            {
                grid.Datatable[0, c] = Datatable[i, c];
            }

            return grid;
        }

        public string[] GetArray(int i)
        {
            string[] arr = new string[LastColumn + 1];

            for (int c = 0; c <= LastColumn; c++)
            {
                arr[c] = plainify(Datatable[i, c]);
            }

            return arr;
        }


        public void Populate(string[,] srcData, DataFrame transformation = null) //, bool keepHeaders = false
        {
            if (!(srcData?.Length > 0)) return;
            Datatable = srcData;
            GetDim(); //keepHeaders
            IndexY();
        }

        public bool Any()
        {
            if (Datatable == null) { return false; }
            return Datatable.GetLength(0) != 0 || Datatable.GetLength(1) != 0;
        }

        private void IndexY()
        {
            throw new NotImplementedException();
        }

        private void GetDim() //bool keepHeaders=false
        {
            if (RowId == null || RowId.Length == 0)
            {
                LastColumn = Datatable.GetLength(0);
                ColumnName = new string[LastColumn];
                ColumnFormat = new string[LastColumn];
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal IData FilterAst(Ast queryParameters)
        {
            //return Filter(queryParameters.children[0]._value, queryParameters.Value, queryParameters.Children[1].Value);
            throw new NotImplementedException();
        }

        public int[] Resize(int rows = 0, int columns = 0)
        {
            if (rows > 0 || columns > 0)
            {
                if (Datatable == null || (Datatable.GetLength(0) == 0 && Datatable.GetLength(1) == 0))
                {
                    if (columns == 0) { columns = 1; }
                    Datatable = new string[rows, columns];
                    ColumnName = new string[columns];
                    ColumnFormat = new string[columns];
                    RowId = new string[rows];
                }
                else
                {
                    string[,] temp = new string[Datatable.GetLength(0) + rows, Datatable.GetLength(1) + columns];
                    int lastRowTemp = Math.Min(Datatable.GetLength(0), temp.GetLength(0));
                    int lastColumnTemp = Math.Min(Datatable.GetLength(1), temp.GetLength(1));

                    string[] tmpColumnName = new string[Datatable.GetLength(1) + columns];
                    for (int i = 0; i < lastColumnTemp; i++)
                    {
                        tmpColumnName[i] = ColumnName[i];
                    }

                    ColumnName = tmpColumnName;

                    for (int i = 0; i < lastRowTemp; i++)
                    {
                        for (int j = 0; j < lastColumnTemp; j++)
                        {
                            temp[i, j] = Datatable[i, j];
                        }

                        //Array.Copy(datatable, i * datatable.GetLength(0), temp, i * temp.GetLength(0), lastColumnTemp);
                    }
                    Datatable = temp;

                }

                LastRow = Datatable.GetLength(0) - 1;
                LastColumn = Datatable.GetLength(1) - 1;
            }
            else if (rows < 0)
            {
                int lastRowTemp = LastRow + rows;
                int lastColumnTemp = LastColumn;

                string[,] temp = new string[lastRowTemp + 1, lastColumnTemp + 1];
                for (int i = 0; i <= lastRowTemp; i++)
                {
                    for (int j = 0; j <= lastColumnTemp; j++)
                    {
                        temp[i, j] = Datatable[i, j];
                    }

                }
                Datatable = temp;
                LastRow = lastRowTemp;
                LastColumn = lastColumnTemp;

            }

            return new[] { LastRow, LastColumn };
        }

        public List<string> Distinct(string fldName)
        {
            List<string> d = new List<string>();
            int j = Col(fldName);
            if (j != -1)
            {
                for (int i = 0; i <= LastRow; i++)
                {
                    if (!d.Contains(Datatable[i, j]))
                    {
                        d.Add(Datatable[i, j]);
                    }
                }
            }
            return d;
        }

        public int Col(string name)
        {
            if (ColumnName != null)
            {
                for (int c = 0; c <= LastColumn; c++)
                {
                    if (ColumnName[c].ToUpper() == name.ToUpper())
                    {
                        return c;
                    }
                }
                return -1;
            }
            else
            {
                return -1;
            }
        }

        public int GetCol(string name)
        {
            int i = Col(name);

            switch (i)
            {
                case -1:
                    Resize(Datatable == null ? 1 : 0, 1);

                    i = LastColumn;
                    ColumnName[i] = name;
                    break;
                    //default:
                    //break;
            }
            return i;
        }


        public IData Filter(string colName, string op, string value)
        {
            int c = Col(colName);
            if (c != -1)
            {
                bool[] evaluator = new bool[LastRow+1];

                try
                {
                    if (op == "==" || op.ToUpper() == "EQ" || op.ToUpper() == "=")
                    {
                        for (int r = 0; r < LastRow + 1; r++)
                        {
                            if (Datatable[r, c] != null)
                            {
                                evaluator[r] = Datatable[r, c].ToUpper() == value.ToUpper();
                            }
                        }
                    }
                    else if (op == "!=" || op == "<>" || op.ToUpper() == "NEQ")
                    {
                        for (int r = 0; r < LastRow; r++)
                        {
                            evaluator[r] = Datatable[r, c].ToUpper() != value.ToUpper();
                        }
                    }
                    else if (op == "<")
                    {
                        if (ColumnFormat[c].ToLower().Contains("date"))
                        {

                            DateTime newDate;
                            DateTime currentDate;

                            for (int r = 0; r <= LastRow; r++)
                            {


                                bool isSourceDate = DateTime.TryParse(value, out newDate);
                                bool isDestDate = DateTime.TryParse(Datatable[r, c], out currentDate);
                                if (isSourceDate)
                                {
                                    if (isDestDate)
                                    {
                                        evaluator[r] = currentDate.CompareTo(newDate)<0;
                                    }
                                    else
                                    {
                                        evaluator[r] = true;
                                    }
                                }
                            }

                            if(!evaluator.Any(t => t == true)) { Datatable = null; }

                        }
                    }

                    else if (op == ">")
                    {
                        if (ColumnFormat[c].ToLower().Contains("date"))
                        {

                            DateTime newDate;
                            DateTime currentDate;

                            for (int r = 0; r <= LastRow; r++)
                            {


                                bool isSourceDate = DateTime.TryParse(value, out newDate);
                                bool isDestDate = DateTime.TryParse(Datatable[r, c], out currentDate);
                                if (isSourceDate)
                                {
                                    if (isDestDate)
                                    {
                                        evaluator[r] = currentDate.CompareTo(newDate) < 0;
                                    }
                                    else
                                    {
                                        evaluator[r] = true;
                                    }
                                }
                            }

                            if (!evaluator.Any(t => t == true)) { Datatable = null; }

                        }
                    }

                    else if (op.ToUpper() == "LIKE")
                    {
                        for (int r = 0; r < LastRow; r++)
                        {
                            evaluator[r] = (Datatable[r, c].Contains(value));
                        }
                    }
                    else if (op.ToUpper() == "OR" || op.ToUpper() == "IN")
                    {
                        string[] matchList = value.Split(',');

                        for (int r = 0; r < LastRow; r++)
                        {
                            evaluator[r] = matchList.Any(m => m == Datatable[r, c]);
                        }
                    }
                }
                catch (Exception ae)
                {
                    Console.WriteLine(ae.InnerException.Message);
                }
                if (evaluator.Any(t => t))              // .Where(t => t == true).Any()
                {

                    int rNew = evaluator.Count(t => t); //.Where(t => t == true)
                    DataFrame filtered = new DataFrame();
                    filtered.Resize(rNew, LastColumn + 1);
                    filtered.ColumnName = ColumnName;
                    filtered.ColumnFormat = ColumnFormat;

                    int fRow = 0;
                    for (int r = 0; r < LastRow+1; r++)
                    {
                        if (evaluator[r])
                        {
                            filtered = CopyRow(this, r, filtered, fRow);
                            fRow++;
                        }
                    }
                    return filtered;
                }
            }
            return null;
        }


        public int[] SmartResize(int? v)
        {
            if (v == null)
            {
                Resize(Ptr - (LastRow + 1), 0);
                return new[] { LastRow, LastColumn };
            }
            else
            {
                if (Ptr >= LastRow-1)
                {
                    Resize(v.GetValueOrDefault(), 0);
                }

                return new[] { Ptr + 1, LastColumn };
            }
        }

        private DataFrame CopyRow(DataFrame src, int rSrc, DataFrame dest, int rDest)
        {
            if (src.Datatable.GetLength(1) == dest.Datatable.GetLength(1))
            {
                for (int c = 0; c <= LastColumn; c++)
                {
                    dest.Datatable[rDest, c] = src.Datatable[rSrc, c];
                }
            }
            return dest;

        }

        public string First(string colName)
        {
            int c = Col(colName.ToUpper());

            if (c != -1)
            {
                if (RowId != null)
                {
                    return Datatable[0, c];
                }
            }
            return null;
        }

        public int FindRow(string fldName, string fldVal, int start = 0)
        {

            if(fldName=="ID" && Col(fldName)==-1)
            {
                int r = -1;
                for (int i = start; i <= LastRow; i++)
                    if (r == -1)
                    {
                        if (RowId[i] == fldVal)
                        {
                            r = i;
                        }
                    }

                return r;


            }
            else
            { 

            int r = -1;
            int c = Col(fldName);

            if (c == -1) return r;

            for (int i = start; i <= LastRow; i++)
                if (r == -1)
                {
                    if (Datatable[i, c] == fldVal)
                    {
                        r = i;
                    }
                }

            return r;
            }
        }

        public void SetValue(int rr, int cc, string value)
        {
            Datatable[rr, cc] = value ?? "";
        }

    public string FindValue(string KeyName)
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string[] v)
        {
            ColumnName = v;
        }

        public void SetRow(int row, string[] v, int ID)
        {
            if (v.Length -1  == LastColumn)
            {
                RowId[row] = ID.ToString();

                int col = 0;
                foreach (var str in v)
                {
                    SetValue(row, col, v[col]);
                    col++;
                }
            }
        }


        public string ToPlainText()
        {
            string result = "";

            if (metadata.Count > 0)
            {
                foreach (var dat in metadata)
                {
                    result = result + getToPlainText(dat.Value, "metadata", dat.Key);
                }
            }

            result = result + getToPlainText(this, "view", Name);

            return result;

        }


        public override string ToString()
        {

            string result = "";

            if(metadata.Count > 0)
            {
                foreach(var dat in metadata)
                {
                    result = result + getToString(dat.Value, "metadata", dat.Key);
                }
            }

            result = result + getToString(this, "view", Name);

            return result;
        }

        private string getToString(DataFrame dat, string dataType, string name)
        {
            string resultString;
            DataFrame meta = dat;

            resultString = "data{{" + "Columns(" + meta.toString(dat.ColumnName) + ")Rows(" + meta.toString(dat.RowId) + ")Data(" + meta.toString(dat.Datatable) + ")}}";
            return dataType + "[<[" + name + "," + resultString + "]>]";
        }


        private string getToPlainText(DataFrame dat, string dataType, string name)
        {
            string resultString;
            DataFrame meta = dat;

            resultString = meta.toPlainText(dat.ColumnName) + "\n" + meta.toPlainText(dat.Datatable);
            return resultString;
        }


        private string toPlainText(string[,] table)
        {
            StringBuilder sb = new StringBuilder();
            for (int ii = 0; ii < LastRow; ii++)
            {
                sb.AppendLine(string.Join("|", GetArray(ii)));
            }

            return sb.ToString();

            //return string.Join("|",
            //        Enumerable.Range(0, table.GetLength(0))
            //           .SelectMany(i => Enumerable.Range(0, table.GetLength(1))
            //                               .Select(j => plainify(table[i, j],j))));
        }

        private string plainify(string inputText)
        {

            if (inputText == null) return "NULL";

            char[] specialCharacters = new char[] { '\r', '\t', '\n', '|' };

            string[] tmp = inputText.Split(specialCharacters, StringSplitOptions.RemoveEmptyEntries);

            return string.Join(" ", tmp);

            //return inputText?.Replace("\n", " ")?.Replace("\r", " ")?.Replace("\t", " ") ?? "NULL";
        }

        private string plainify(string inputText, int col)
        {
            string tmp = inputText?.Replace("\n", " ")?.Replace("\r", " ")?.Replace("\t", " ") ?? "NULL";
            if (col == LastColumn) tmp = tmp + "\n";

            return tmp;
        }

        private string toPlainText(string[] table)
        {
            if (table != null)
            {
                return string.Join(("|"), Enumerable.Range(0, table.GetLength(0))
                                              .Select(j => table[j]));
            }
            else
            {
                return null;
            }
        }


        private string toString(string[,] table)
        {
            return string.Join("|",
                    Enumerable.Range(0, table.GetLength(0))
                       .SelectMany(i => Enumerable.Range(0, table.GetLength(1))
                                           .Select(j => Encode.encodeValue(_value: table[i, j], _destination: "API"))));
        }

        private string toString(string[] table)
        {
            if (table != null)
            {
                return string.Join(("|"), Enumerable.Range(0, table.GetLength(0))
                                              .Select(j => Encode.encodeValue(_value: table[j], _destination: "API")));
            }
            else
            {
                return null;
            }
        }

        public void index()
        {
            int indexcolumn = Col("ID");
            if(indexcolumn != -1)
            {
                RowId = new string[Datatable.GetLength(0)];
                for(int i = 0; i < Datatable.GetLength(0); i++)
                {
                    RowId[i] = Datatable.GetValue(i, indexcolumn).ToString();
                }
            }
        }
    }
}
