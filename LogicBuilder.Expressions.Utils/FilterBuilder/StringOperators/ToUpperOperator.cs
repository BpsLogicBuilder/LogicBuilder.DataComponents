﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.StringOperators
{
    public class ToUpperOperator : FilterPart
    {
        public ToUpperOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build()
        {
            Expression operandExpression = Operand.Build();

            if (operandExpression.Type == typeof(string))
                return operandExpression.GetStringToUpperCall();
            else
                throw new ArgumentException(nameof(Operand));
        }
    }
}