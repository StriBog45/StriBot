using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriBot
{
    public class CollectionHelper : List<String>
    {
        public override string ToString()
        {
            return string.Join(", ", this);
        }
        public string ToString(string Separator)
        {
            return string.Join(Separator, this);
        }
    }
}
