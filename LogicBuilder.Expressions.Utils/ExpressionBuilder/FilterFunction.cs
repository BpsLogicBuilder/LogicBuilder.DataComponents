namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
	public enum FilterFunction
    {
	//Logical
		eq,
		ne,
		gt,
		ge,
		lt,
		le,
		and,
		or,
		not,
		has,
		@in,

	//Arithmetic
		add,
		sub,
		negation,
		mul,
		div,
		mod,

	//Cacnonical
		concat,
		contains,
		endswith,
		indexof,
		length,
		startswith,
		substring,

	//Collection
		hassubset,
		hassubsequence,

	//String
		tolower,
		toupper,
		trim,

	//DateTime
		date,
		day,
		fractionalseconds,
		hour,
		maxdatetime,
		mindatetime,
		minute,
		month,
		now,
		second,
		time,
		totaloffsetminutes,
		totalseconds,
		year,

	//Arithmetic
		ceiling,
		floor,
		round,

	//Type
		cast,
		isof,

	//Lambda
		any,
		all,
	}
}
