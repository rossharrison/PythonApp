using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interface
{
    public interface IObject
    {
        string name { get; set; }
        string type { get; set; }
        object target { get; set; }
        Ast environment { get; set; }
        void build();
    }
}

