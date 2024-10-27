using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(nameof(DerivedThing.Id), typeof(DerivedThing))]
        [InlineData(nameof(DerivedThing.Name), typeof(BaseThing))]
        [InlineData(nameof(DerivedThing.DataInBytes), typeof(BaseThing))]
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
        [InlineData(nameof(DerivedThing.DataInBytes), typeof(BaseThing))]
        [InlineData(nameof(DerivedThing.Description), typeof(DerivedThing))]
        public void MemberInfoReflectedTypeMustMatchTheDeclaringTypeForGetSelectedMembers(string propertyName,
            Type reflectedType)
        {
            //act
            MemberInfo memberInfo = typeof(DerivedThing).GetSelectedMembers(new List<string>())
                .FirstOrDefault(m => m.Name == propertyName);

            //assert
            Assert.Equal(reflectedType.FullName, memberInfo.ReflectedType.FullName);
        }

        [Theory]
        [InlineData(typeof(DerivedThing))]
        [InlineData(typeof(BaseThing))]
        public void GetSelectedMembers_WhenSelectIsEmpty_MustReturnAllLiteralMembers(Type reflectedType)
        {
            // Act
            var memberInfos = reflectedType.GetSelectedMembers(Enumerable.Empty<string>().ToList());

            // Assert
            Assert.Multiple(() =>
            {
                var names = memberInfos.Select(mi => mi.Name).ToList();
                
                Assert.DoesNotContain(memberInfos, name => name.Name == nameof(BaseThing.Objects));
                
                Assert.Contains(names, name => name == nameof(BaseThing.ParametersArray));
                Assert.Contains(names, name => name == nameof(BaseThing.ParametersList));
                Assert.Contains(names, name => name == nameof(BaseThing.Ints));
                Assert.Contains(names, name => name == nameof(BaseThing.Strings));
                Assert.Contains(names, name => name == nameof(BaseThing.Booleans));
                Assert.Contains(names, name => name == nameof(BaseThing.DateTimes));
                Assert.Contains(names, name => name == nameof(BaseThing.Dates));
                Assert.Contains(names, name => name == nameof(BaseThing.Guides));
                Assert.Contains(names, name => name == nameof(BaseThing.UnsignedInts));
            });
        }

        private abstract class BaseThing
        {
            public string Name { get; set; }
            public byte[] DataInBytes { get; set; }
            public string[] ParametersArray { get; set; }
            public ICollection<string> Strings { get; set; }
            public List<string> ParametersList { get; set; }
            public List<bool> Booleans { get; set; }
            public ISet<DateTime> DateTimes { get; set; }
            public ISet<DateOnly> Dates { get; set; }
            public HashSet<Guid> Guides { get; set; }
            public uint[] UnsignedInts { get; set; }
            public IEnumerable<int> Ints { get; set; }
            public List<object> Objects { get; set; }
        }

        private class DerivedThing : BaseThing, IDerivedThing
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
        }

        private interface IDerivedThing
        {
            public string Description { get; set; }
        }
    }
}