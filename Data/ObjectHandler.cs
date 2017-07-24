using Data.Interface;

namespace Data
{
    public class ObjectHandler : IObject
    {
        public string name { get; set; }
        public string type { get; set; }
        public object target { get; set; }
        public Ast environment { get; set; } = new Ast("environment");

        public ObjectHandler(string _name, object _target)
        {
            name = _name;
            type = _target.GetType().ToString();
            target = _target;
        }

        public ObjectHandler(string _name, string _type, object _target)
        {
            name = _name;
            type = _type;
            target = _target;
        }

        public void build()
        {
        }
    }
}
