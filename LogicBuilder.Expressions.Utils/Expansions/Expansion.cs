using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    abstract public class Expansion
    {
        public string MemberName { get; set; }
        public Type MemberType { get; set; }
        public Type ParentType { get; set; }
        public List<string> Selects { get; set; }
    }
}
