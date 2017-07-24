using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interface
{
    public interface ITerm
    {
        string name { get; set; }
        object value { get; set; }
        object GetValue(string name);  // if singleton or sequence

        bool HasValue(string name); // does the slot exist

        void SetValue(string name, object value); // we should have specializations for slottype and int!
                                                    // the type will have info on slots or external use!

        ISlot GetSlot(string name);
        //

    }
}
