using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnDATAGRID.Utils
{
    public class Filter
    {
        public string NameClass { get; set; }
        public object Instance { get; set; }

        public Filter(string nameClass)
        {
            object[] args = { false };

            NameClass = nameClass;
            Type type = Type.GetType(NameClass);
            Instance = Activator.CreateInstance(type, args);
        }
    }
}
