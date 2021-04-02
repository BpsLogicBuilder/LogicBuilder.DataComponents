using LogicBuilder.Domain;
using System;

namespace Contoso.Domain
{
    abstract public class BaseModelClass : BaseModel
    {
        public string TypeFullName => this.GetType().ToTypeString();
    }

    public static class TypeHelpers
    {
        public static string ToTypeString(this Type type)
            => type.IsGenericType && !type.IsGenericTypeDefinition
                ? type.AssemblyQualifiedName
                : type.FullName;
    }
}
