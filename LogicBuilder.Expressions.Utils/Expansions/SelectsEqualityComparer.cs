using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    internal class SelectsEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) => string.Compare(x, y, true) == 0;

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
