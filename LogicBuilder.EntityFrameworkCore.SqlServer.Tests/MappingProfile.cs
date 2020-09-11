using AutoMapper;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Conversions;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.StringOperators;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Tests
{
	public class MappingProfile : Profile
	{
		const string PARAMETERS_KEY = "parameters";

		public MappingProfile()
		{
			CreateMap<AddBinaryDescriptor, AddBinaryOperator>();
			CreateMap<AllDescriptor, AllOperator>()
				.ConstructUsing
				(
					(src, context) => new AllOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<AndBinaryDescriptor, AndBinaryOperator>();
			CreateMap<AnyDescriptor, AnyOperator>()
				.ConstructUsing
				(
					(src, context) => new AnyOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<AsQueryableDescriptor, AsQueryableOperator>();
			CreateMap<AverageDescriptor, AverageOperator>()
				.ConstructUsing
				(
					(src, context) => new AverageOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<BinaryDescriptor, BinaryOperator>();
			CreateMap<CastDescriptor, CastOperator>();
			CreateMap<CeilingDescriptor, CeilingOperator>();
			CreateMap<CollectionCastDescriptor, CollectionCastOperator>();
			CreateMap<CollectionConstantDescriptor, CollectionConstantOperator>();
			CreateMap<ConcatDescriptor, ConcatOperator>();
			CreateMap<ConstantDescriptor, ConstantOperator>();
			CreateMap<ContainsDescriptor, ContainsOperator>();
			CreateMap<ConvertCharArrayToStringDescriptor, ConvertCharArrayToStringOperator>();
			CreateMap<ConvertDescriptor, ConvertOperator>();
			CreateMap<ConvertToEnumDescriptor, ConvertToEnumOperator>();
			CreateMap<ConvertToNullableUnderlyingValueDescriptor, ConvertToNullableUnderlyingValueOperator>();
			CreateMap<ConvertToNumericDateDescriptor, ConvertToNumericDateOperator>();
			CreateMap<ConvertToNumericTimeDescriptor, ConvertToNumericTimeOperator>();
			CreateMap<ConvertToStringDescriptor, ConvertToStringOperator>();
			CreateMap<CountDescriptor, CountOperator>()
				.ConstructUsing
				(
					(src, context) => new CountOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<CustomMethodDescriptor, CustomMethodOperator>();
			CreateMap<DateDescriptor, DateOperator>();
			CreateMap<DayDescriptor, DayOperator>();
			CreateMap<DistinctDescriptor, DistinctOperator>();
			CreateMap<DivideBinaryDescriptor, DivideBinaryOperator>();
			CreateMap<EndsWithDescriptor, EndsWithOperator>();
			CreateMap<EqualsBinaryDescriptor, EqualsBinaryOperator>();
			CreateMap<FilterLambdaDescriptor, FilterLambdaOperator>()
				.ConstructUsing
				(
					(src, context) => new FilterLambdaOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.Selector),
						src.SourceElementType,
						src.ParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<FirstDescriptor, FirstOperator>()
				.ConstructUsing
				(
					(src, context) => new FirstOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<FirstOrDefaultDescriptor, FirstOrDefaultOperator>()
				.ConstructUsing
				(
					(src, context) => new FirstOrDefaultOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<FloorDescriptor, FloorOperator>();
			CreateMap<FractionalSecondsDescriptor, FractionalSecondsOperator>();
			CreateMap<GreaterThanBinaryDescriptor, GreaterThanBinaryOperator>();
			CreateMap<GreaterThanOrEqualsBinaryDescriptor, GreaterThanOrEqualsBinaryOperator>();
			CreateMap<GroupByDescriptor, GroupByOperator>()
				.ConstructUsing
				(
					(src, context) => new GroupByOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<HasDescriptor, HasOperator>();
			CreateMap<HourDescriptor, HourOperator>();
			CreateMap<IEnumerableSelectorLambdaDescriptor, IEnumerableSelectorLambdaOperator>()
				.ConstructUsing
				(
					(src, context) => new IEnumerableSelectorLambdaOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.Selector),
						src.SourceElementType,
						src.ParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<IndexOfDescriptor, IndexOfOperator>();
			CreateMap<InDescriptor, InOperator>();
			CreateMap<IsOfDescriptor, IsOfOperator>();
			CreateMap<LastDescriptor, LastOperator>()
				.ConstructUsing
				(
					(src, context) => new LastOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<LastOrDefaultDescriptor, LastOrDefaultOperator>()
				.ConstructUsing
				(
					(src, context) => new LastOrDefaultOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<LengthDescriptor, LengthOperator>();
			CreateMap<LessThanBinaryDescriptor, LessThanBinaryOperator>();
			CreateMap<LessThanOrEqualsBinaryDescriptor, LessThanOrEqualsBinaryOperator>();
			CreateMap<MaxDateTimeDescriptor, MaxDateTimeOperator>();
			CreateMap<MaxDescriptor, MaxOperator>()
				.ConstructUsing
				(
					(src, context) => new MaxOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<MemberInitDescriptor, MemberInitOperator>();
			CreateMap<MemberSelectorDescriptor, MemberSelectorOperator>();
			CreateMap<MinDateTimeDescriptor, MinDateTimeOperator>();
			CreateMap<MinDescriptor, MinOperator>()
				.ConstructUsing
				(
					(src, context) => new MinOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<MinuteDescriptor, MinuteOperator>();
			CreateMap<ModuloBinaryDescriptor, ModuloBinaryOperator>();
			CreateMap<MonthDescriptor, MonthOperator>();
			CreateMap<MultiplyBinaryDescriptor, MultiplyBinaryOperator>();
			CreateMap<NegateDescriptor, NegateOperator>();
			CreateMap<NotEqualsBinaryDescriptor, NotEqualsBinaryOperator>();
			CreateMap<NotDescriptor, NotOperator>();
			CreateMap<NowDateTimeDescriptor, NowDateTimeOperator>();
			CreateMap<OrBinaryDescriptor, OrBinaryOperator>();
			CreateMap<OrderByDescriptor, OrderByOperator>()
				.ConstructUsing
				(
					(src, context) => new OrderByOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SortDirection,
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<ParameterDescriptor, ParameterOperator>()
				.ConstructUsing
				(
					(src, context) => new ParameterOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						src.ParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<RoundDescriptor, RoundOperator>();
			CreateMap<SecondDescriptor, SecondOperator>();
			CreateMap<SelectManyDescriptor, SelectManyOperator>()
				.ConstructUsing
				(
					(src, context) => new SelectManyOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<SelectDescriptor, SelectOperator>()
				.ConstructUsing
				(
					(src, context) => new SelectOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<SelectorLambdaDescriptor, SelectorLambdaOperator>()
				.ConstructUsing
				(
					(src, context) => new SelectorLambdaOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.Selector),
						src.SourceElementType,
						src.BodyType,
						src.ParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<SingleDescriptor, SingleOperator>()
				.ConstructUsing
				(
					(src, context) => new SingleOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<SingleOrDefaultDescriptor, SingleOrDefaultOperator>()
				.ConstructUsing
				(
					(src, context) => new SingleOrDefaultOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<SkipDescriptor, SkipOperator>();
			CreateMap<StartsWithDescriptor, StartsWithOperator>();
			CreateMap<SubstringDescriptor, SubstringOperator>();
			CreateMap<SubtractBinaryDescriptor, SubtractBinaryOperator>();
			CreateMap<SumDescriptor, SumOperator>()
				.ConstructUsing
				(
					(src, context) => new SumOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<TakeDescriptor, TakeOperator>();
			CreateMap<ThenByDescriptor, ThenByOperator>()
				.ConstructUsing
				(
					(src, context) => new ThenByOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.SelectorBody),
						src.SortDirection,
						src.SelectorParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<TimeDescriptor, TimeOperator>();
			CreateMap<ToListDescriptor, ToListOperator>();
			CreateMap<ToLowerDescriptor, ToLowerOperator>();
			CreateMap<TotalOffsetMinutesDescriptor, TotalOffsetMinutesOperator>();
			CreateMap<TotalSecondsDescriptor, TotalSecondsOperator>();
			CreateMap<ToUpperDescriptor, ToUpperOperator>();
			CreateMap<TrimDescriptor, TrimOperator>();
			CreateMap<WhereDescriptor, WhereOperator>()
				.ConstructUsing
				(
					(src, context) => new WhereOperator
					(
						(IDictionary<string, ParameterExpression>)context.Items[PARAMETERS_KEY],
						context.Mapper.Map<IExpressionPart>(src.SourceOperand),
						context.Mapper.Map<IExpressionPart>(src.FilterBody),
						src.FilterParameterName
					)
				)
				.ForAllMembers(opt => opt.Ignore());

			CreateMap<YearDescriptor, YearOperator>();

			CreateMap<IExpressionDescriptor, IExpressionPart>()
				.Include<AddBinaryDescriptor, AddBinaryOperator>()
				.Include<AllDescriptor, AllOperator>()
				.Include<AndBinaryDescriptor, AndBinaryOperator>()
				.Include<AnyDescriptor, AnyOperator>()
				.Include<AsQueryableDescriptor, AsQueryableOperator>()
				.Include<AverageDescriptor, AverageOperator>()
				.Include<BinaryDescriptor, BinaryOperator>()
				.Include<CastDescriptor, CastOperator>()
				.Include<CeilingDescriptor, CeilingOperator>()
				.Include<CollectionCastDescriptor, CollectionCastOperator>()
				.Include<CollectionConstantDescriptor, CollectionConstantOperator>()
				.Include<ConcatDescriptor, ConcatOperator>()
				.Include<ConstantDescriptor, ConstantOperator>()
				.Include<ContainsDescriptor, ContainsOperator>()
				.Include<ConvertCharArrayToStringDescriptor, ConvertCharArrayToStringOperator>()
				.Include<ConvertDescriptor, ConvertOperator>()
				.Include<ConvertToEnumDescriptor, ConvertToEnumOperator>()
				.Include<ConvertToNullableUnderlyingValueDescriptor, ConvertToNullableUnderlyingValueOperator>()
				.Include<ConvertToNumericDateDescriptor, ConvertToNumericDateOperator>()
				.Include<ConvertToNumericTimeDescriptor, ConvertToNumericTimeOperator>()
				.Include<ConvertToStringDescriptor, ConvertToStringOperator>()
				.Include<CountDescriptor, CountOperator>()
				.Include<CustomMethodDescriptor, CustomMethodOperator>()
				.Include<DateDescriptor, DateOperator>()
				.Include<DayDescriptor, DayOperator>()
				.Include<DistinctDescriptor, DistinctOperator>()
				.Include<DivideBinaryDescriptor, DivideBinaryOperator>()
				.Include<EndsWithDescriptor, EndsWithOperator>()
				.Include<EqualsBinaryDescriptor, EqualsBinaryOperator>()
				.Include<FilterLambdaDescriptor, FilterLambdaOperator>()
				.Include<FirstDescriptor, FirstOperator>()
				.Include<FirstOrDefaultDescriptor, FirstOrDefaultOperator>()
				.Include<FloorDescriptor, FloorOperator>()
				.Include<FractionalSecondsDescriptor, FractionalSecondsOperator>()
				.Include<GreaterThanBinaryDescriptor, GreaterThanBinaryOperator>()
				.Include<GreaterThanOrEqualsBinaryDescriptor, GreaterThanOrEqualsBinaryOperator>()
				.Include<GroupByDescriptor, GroupByOperator>()
				.Include<HasDescriptor, HasOperator>()
				.Include<HourDescriptor, HourOperator>()
				.Include<IEnumerableSelectorLambdaDescriptor, IEnumerableSelectorLambdaOperator>()
				.Include<IndexOfDescriptor, IndexOfOperator>()
				.Include<InDescriptor, InOperator>()
				.Include<IsOfDescriptor, IsOfOperator>()
				.Include<LastDescriptor, LastOperator>()
				.Include<LastOrDefaultDescriptor, LastOrDefaultOperator>()
				.Include<LengthDescriptor, LengthOperator>()
				.Include<LessThanBinaryDescriptor, LessThanBinaryOperator>()
				.Include<LessThanOrEqualsBinaryDescriptor, LessThanOrEqualsBinaryOperator>()
				.Include<MaxDateTimeDescriptor, MaxDateTimeOperator>()
				.Include<MaxDescriptor, MaxOperator>()
				.Include<MemberInitDescriptor, MemberInitOperator>()
				.Include<MemberSelectorDescriptor, MemberSelectorOperator>()
				.Include<MinDateTimeDescriptor, MinDateTimeOperator>()
				.Include<MinDescriptor, MinOperator>()
				.Include<MinuteDescriptor, MinuteOperator>()
				.Include<ModuloBinaryDescriptor, ModuloBinaryOperator>()
				.Include<MonthDescriptor, MonthOperator>()
				.Include<MultiplyBinaryDescriptor, MultiplyBinaryOperator>()
				.Include<NegateDescriptor, NegateOperator>()
				.Include<NotEqualsBinaryDescriptor, NotEqualsBinaryOperator>()
				.Include<NotDescriptor, NotOperator>()
				.Include<NowDateTimeDescriptor, NowDateTimeOperator>()
				.Include<OrBinaryDescriptor, OrBinaryOperator>()
				.Include<OrderByDescriptor, OrderByOperator>()
				.Include<ParameterDescriptor, ParameterOperator>()
				.Include<RoundDescriptor, RoundOperator>()
				.Include<SecondDescriptor, SecondOperator>()
				.Include<SelectManyDescriptor, SelectManyOperator>()
				.Include<SelectDescriptor, SelectOperator>()
				.Include<SelectorLambdaDescriptor, SelectorLambdaOperator>()
				.Include<SingleDescriptor, SingleOperator>()
				.Include<SingleOrDefaultDescriptor, SingleOrDefaultOperator>()
				.Include<SkipDescriptor, SkipOperator>()
				.Include<StartsWithDescriptor, StartsWithOperator>()
				.Include<SubstringDescriptor, SubstringOperator>()
				.Include<SubtractBinaryDescriptor, SubtractBinaryOperator>()
				.Include<SumDescriptor, SumOperator>()
				.Include<TakeDescriptor, TakeOperator>()
				.Include<ThenByDescriptor, ThenByOperator>()
				.Include<TimeDescriptor, TimeOperator>()
				.Include<ToListDescriptor, ToListOperator>()
				.Include<ToLowerDescriptor, ToLowerOperator>()
				.Include<TotalOffsetMinutesDescriptor, TotalOffsetMinutesOperator>()
				.Include<TotalSecondsDescriptor, TotalSecondsOperator>()
				.Include<ToUpperDescriptor, ToUpperOperator>()
				.Include<TrimDescriptor, TrimOperator>()
				.Include<WhereDescriptor, WhereOperator>()
				.Include<YearDescriptor, YearOperator>();
		}
	}
}
