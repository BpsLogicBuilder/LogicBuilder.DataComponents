namespace LogicBuilder.Expressions.Utils
{
    public struct AttributeNames
    {
        public const string DEFAULTVALUE = "DefaultValue";
        public const string USEFOREQUALITY = "UseForEquality";
        public const string USEFORHASHCODE = "UseForHashCode";
        public const string USEFORTOSTRING = "UseForToString";
        public const string PROPERTYSOURCE = "propertySource";
        public const string PROPERTYSOURCEPARAMETER = "propertySourceParameter";
    }

    public struct UnreferencedLiteralTypeNames
    {
        public const string DATEONLY = "System.DateOnly";
        public const string TIMEONLY = "System.TimeOnly";
        public const string DATE = "Microsoft.OData.Edm.Date";
        public const string TIMEOFDAY = "Microsoft.OData.Edm.TimeOfDay";
    }
}
