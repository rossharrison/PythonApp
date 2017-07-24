using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Encoding
{
    public static class Encode
    {
        public static string value;
        public static bool valid = false;


        public static string encodeValue(string _value, string _format = "Text", string _destination = "SQL Server")
        {
            switch (_destination)
            {
                case "SQL Server":
                    SQLEncode(_value, _format);
                    break;
                case "SharePoint":
                    SPEncode(_value, _format);
                    break;
                case "API":
                    APIEncode(_value, _format);
                    break;
            }

            if(valid)
            {
                return value;
            }
            else
            {
                if(_destination=="API")
                {
                    return Base64Encode("NULL");
                }
                else
                {
                    return "NULL";
                }
            }

        }


        //public static Encode(string _value, string _format, string _destination="SQL Server")
        //{
        //    switch(_destination)
        //    {
        //        case "SQL Server":
        //            SQLEncode(_value, _format);
        //            break;
        //        case "SharePoint":
        //            SPEncode(_value, _format);
        //            break;
        //        case "API":
        //            APIEncode(_value, _format);
        //            break;
        //    }
        //}


        public static void SQLEncode(string _value, string _format)
        {
            string __value = Base64Decode(_value);

            if (__value == null || __value.ToUpper().Contains("NULL") || __value == "")
            {
                value = "NULL";
                valid = true;
            }
            else
            {



                switch (_format)
                {
                    case "Text":
                    case "Notes":
                            value = "'" + __value.Replace('\'', '"') + "'";
                            valid = true;
                            break;

                    case "Number":
                            int i;

                            if (Int32.TryParse(__value, out i))
                            {
                                valid = true;
                                value = i.ToString();
                            }
                            else
                            {
                                valid = false;
                            }
                            break;

                    case "Date Time":
                    case "Date":
                    case "Time":
                        DateTime date;
                        Double dbl;

                        if (DateTime.TryParse(__value, out date))
                        {
                            value = "'" + date.ToString("YYYY-MM-DD HH24:MI:SS.mmm") + "'";
                            valid = true;
                        }
                        else if (Double.TryParse(__value, out dbl))
                        {
                            value = "'" + (DateTime.FromOADate(dbl)).ToString("yyyy-MM-dd hh:mm:ss.mmm") + "'";
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                        }

                        break;


                    case "Currency":

                        NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
                        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");

                        if (Double.TryParse(__value, style, culture, out dbl))
                        {
                            value = dbl.ToString("F");
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                        }
                        break;
                    default:
                        value = "'" + __value.Replace('\'', '"') + "'";
                        valid = true;
                        break;
                }
            }
        }

        public static void APIEncode(string _value, string _format)
        {
            switch(_format)
            {
                case "Date Time":
                case "Date":
                case "Time":
                    DateTime date;
                    if (DateTime.TryParse(_value, out date))
                    {
                        value = Convert.ToDouble(date).ToString();
                        valid = true;
                    }
                    else
                    {
                        value = "NULL";
                        valid = false;

                    }
                    break;
                case "Currency":
                    Decimal dec;
                    if (Decimal.TryParse(_value, out dec))
                    {
                        valid = true;
                        value = dec.ToString("F");
                    }
                    else
                    {
                        valid = false;
                        return;
                    }
                    break;
                default:
                    if (_value == "" || _value == null || _value.ToUpper() == "'NULL'" || _value.ToUpper() == "NULL")
                    {
                        value = "NULL";
                    }
                    else
                    {
                        valid = true;
                        value = _value.Replace('\'', '"');
                    }
                    break;
            }

                value = Base64Encode(value);

        }
        public static string Base64Encode(string plainText)
        {
            if (plainText == null)
            {
                return "";
            }
            else
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void SPEncode(string _value, string _format)
        {
            throw new NotImplementedException();
        }
    }
}

