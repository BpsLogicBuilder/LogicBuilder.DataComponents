using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(nameof(DerivedThing.Id), typeof(DerivedThing))]
        [InlineData(nameof(DerivedThing.Name), typeof(BaseThing))]
        [InlineData(nameof(DerivedThing.Description), typeof(DerivedThing))]
        public void MemberInfoReflectedTypeMustMatchTheDeclaringType(string propertyName, Type reflectedType)
        {
            //act
            MemberInfo memberInfo = typeof(DerivedThing).GetMemberInfo(propertyName);

            //assert
            Assert.Equal(reflectedType.FullName, memberInfo.ReflectedType.FullName);
        }

        [Theory]
        [InlineData(nameof(DerivedThing.Id), typeof(DerivedThing))]
        [InlineData(nameof(DerivedThing.Name), typeof(BaseThing))]
        [InlineData(nameof(DerivedThing.Description), typeof(DerivedThing))]
        public void MemberInfoReflectedTypeMustMatchTheDeclaringTypeForGetSelectedMembers(string propertyName, Type reflectedType)
        {
            //act
            MemberInfo memberInfo = typeof(DerivedThing).GetSelectedMembers(new List<string>()).FirstOrDefault(m => m.Name == propertyName);

            //assert
            Assert.Equal(reflectedType.FullName, memberInfo.ReflectedType.FullName);
        }

        public abstract class BaseThing
        {
            public string Name { get; set; }
        }

        public class DerivedThing : BaseThing, IDerivedThing
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        public interface IDerivedThing
        {
            public string Description { get; set; }
        }
    }
}
