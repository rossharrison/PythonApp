using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interface
{
    public enum ChangeType { Delete, Create, Change };

    public delegate void Change(ISlot source, ChangeType changeType, object newValue, object oldValue);

    public interface ISlot
    {
        void SetValue(object value); // alternative for Insert, for a collection maybe should be entire collection
                                     // this will work for variables. is an operand a slot?
        object GetValue();  // coresponds to getvalue(columnname) on ITerm
                            // we could autocoerce types
                            // a lot might be collection of ITerms 
                            //
                            // only slots can 
        ITerm Insert(object value = null);  // we will use for type create
                                            // types should
                                            //
        ITerm Parent { get; }
        //
        // we need a changed 
        event Change Changes;
        //
    }
}
