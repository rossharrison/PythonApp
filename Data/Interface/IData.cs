using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interface
{
    public interface IData
    {
        string type { get; set; }
        string CRUDtype { get; set; }
        string FindValue(string KeyName);
    }
}
