using Contoso.Data.Entities;
using LogicBuilder.Expressions.Utils.Expansions;
using System.Collections.Generic;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class BuildExpansionsTests
    {
        [Fact]
        public void ShouldNotThrowifExpandedMemberIsLiteral()
        {
            var expression = new SelectExpandDefinition
            {
                ExpandedItems = new List<SelectExpandItem>
                {
                    new SelectExpandItem
                    {
                        MemberName = "Name"
                    }
                }
            }.GetExpansionSelectors<Department>();
        }
    }
}
