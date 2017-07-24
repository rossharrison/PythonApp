using System;
using Data.Interface;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace Data
{
    public class Ast : ITerm, IData
    {
        public string type { get;set; } = "Ast";
        public string CRUDtype { get; set; }
        // 
        public object value { get; set; } // for terminals
        //
        // also we can freeze for futher use!
        // need our input, could make thread local!
        //

        public string decodedValue()
        {
            Type val = value.GetType();
            
            if(val.ToString().ToLower() == "string")
            {
                try { 
                return Encoding.Encode.Base64Decode((string)value);
                }
                catch
                {
                    return (string)value;
                }
            }
            else
            {
                return "[]";
            }
        }

        public List<ITerm> children; // lazy initiiate // change to dictionary?
                                     //

        public bool IsTop
        {
            get; set;
        }

        public int left
        {
            get; set;
        }

        public int right
        {
            get; set;
        }

        public string name
        {
            get; set;
        }


        public List<ITerm> Children
        {
            get
            {
                return children;
            }

            set
            {
                if (value != null)
                {
                    children = value;
                    foreach (Ast child in children)
                    {
                        child.parent = this;
                    }
                }
            }
        }

        public Ast parent
        {
            get;

            set;

        }

        public Ast(string name = "")
        {
            this.name = name;
        }

        public Ast(string name, object value) : this(name)
        {
            this.value = value;
            Ast v = value as Ast;
            if (v != null)
            {
                v.parent = this;
                if (name != "Body")
                    Debugger.Break();
            }
        }

        public Ast getFirstChild(string name)
        {
            return (Ast)children?.Where(t => t.name.ToLower() == name.ToLower())?.First();
        }

        public void Delete(string name)
        {
            foreach (var child in children)
            {
                if (child.name == name)
                    children.Remove(child);
            }
        }





        public Ast Add(Ast right)
        {
            if (right != null)
            {
                Debug.Assert(this != right);
                if (IsTop)
                {
                    Ast res = new Ast();
                    res.Add(this);
                    res.Add(right);
                    return res;
                }
                else
                {
                    if (children == null)
                        children = new List<ITerm>();
                    children.Add((Ast)right);
                    right.parent = this;
                    return this;
                }
            }
            else
                return this;
        }

        public override string ToString()
        {
            // Debugger.Break();
            return name;
        }

        public string Display(string input)
        {
            StringBuilder sb = new StringBuilder();
            //
            int indent = 0;
            Display(input, sb, indent);
            //
            return sb.ToString();
        }

        public void Display(string input, StringBuilder sb, int indent)
        {
            // your parent will already have positioned you, you
            // need to position your children and restore the indent
            int start = indent;

            sb.Append(name);
            sb.Append("(");
            indent += ((name == null) ? 0 : name.Length) + 1;
            if (children == null || children.Count == 0) // terminal, just use left and right
            {
                if (value != null)
                    sb.Append(value);
                else
                {
                    int len = right - left;
                    if (len > input.Length)
                        len = input.Length - 1;
                    if (len > 0)
                        sb.Append(input.Substring(left, len));
                    else
                        sb.Append("-");
                }
                sb.Append(")");
            }
            else
            {
                // allways display the first child
                ((Ast)children[0]).Display(input, sb, indent); // append to this line
                if (children.Count > 1)
                {
                    sb.Append(",\n");
                    for (int i = 1; i < children.Count; i++)
                    {
                        // parent adds the spaces
                        sb.Append(MakeSpaces(indent));
                        ((Ast)children[i]).Display(input, sb, indent);
                        if (i < children.Count - 1)
                            sb.Append(",");
                        sb.Append("\n");
                    }
                    sb.Append(MakeSpaces(indent));
                    sb.Append(")"); // parent will add linefeed
                }
                else
                    sb.Append(")");
            }
        }

        string MakeSpaces(int size)
        {
            return "".PadRight(size);
        }


        public void Flatten()
        {
            if (children != null)
            {
                // first tell all children to flatten
                foreach (Ast ast in children)
                    ast.Flatten();
                //
                bool tagless = false;
                foreach (Ast ast in Children)
                {
                    tagless = tagless || (ast.name == null || ast.name.Equals(""));
                }
                if (tagless)
                {
                    // need to flatten
                    List<ITerm> l = new List<ITerm>();
                    foreach (Ast ast in children)
                    {
                        if (ast.name == null || ast.name.Equals(""))
                        {
                            foreach (Ast cast in ast.Children)
                                l.Add(cast);
                        }
                        else
                            l.Add(ast);
                    }
                    children = l;
                }
            }
        }

        public void MakeStrings(string input)
        {
            if (children == null || children.Count == 0)
                value = input.Substring(left, right - left);
            else
                foreach (Ast ast in children)
                    ast.MakeStrings(input);
        }

        #region ITerm
        public object GetValue(string name)
        {
            // we need to find the child ast with this name
            // and convert the value to type T
            switch (name)
            {
                case "Tag":
                    return this.name;
                case "Value":
                    return value;
                case "First":
                    return children[0];
                case "Children":
                    return children;
                case "Left":
                    return left;
                case "Right":
                    return right;
                default:

                    Ast res = (Ast)Find(name);
                    if (res == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (res.value != null)
                            return res.value;
                        else
                        {
                            return res;
                        }
                    }
            }
        }


        public bool HasValue(string name)
        {
            /*
            if (name == "Name")
                return false;
            else if (name == "Children")
                if (Children != null)
                    return Children.Count > 0;
                else
                {
                    return false;
                }
            else
                */
            return GetValue(name) != null; // this should be done explicitly
        }



        #endregion

        #region ISlot


        public void SetValue(object value)
        {

            this.value = value;
        }



        #endregion


        public Ast Find(string name)
        {
            if (children == null)
                return null;
            else
            {
                for (int i = 0; i < children.Count; i++)
                    if (((string)children[i].GetValue("Tag")).Equals(name))
                        return (Ast)children[i];
                return null;

            }
        }

        public string FindValue(string name)
        {
            if (children == null)
                return null;
            else
            {
                for (int i = 0; i < children.Count; i++)
                    if (((string)children[i].GetValue("Tag")).Equals(name))
                        return children[i].ToString();
                return null;

            }
        }


        public void AddChild(Ast child)
        {
            if (children == null)
                Children = new List<ITerm>();
            children.Add(child);
            child.parent = this;
        }

        public void SetValue(string name, object value)
        {
            Ast ch = new Ast(name);
            ch.value = value;
            AddChild(ch);
        }

        public ISlot GetSlot(string name)  // children should be slot
        {
            throw new NotImplementedException();
        }

    }
}
