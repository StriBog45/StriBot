using System.Collections.Generic;

namespace StriBot
{
    public class CollectionHelper : List<string>
    {
        public override string ToString()
            => string.Join(", ", this);

        public string ToString(string Separator)
            => string.Join(Separator, this);
    }
}