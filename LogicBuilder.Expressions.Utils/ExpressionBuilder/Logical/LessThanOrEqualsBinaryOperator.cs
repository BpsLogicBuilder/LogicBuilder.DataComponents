﻿using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class LessThanOrEqualsBinaryOperator : BinaryOperator
    {
        public LessThanOrEqualsBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
            BinaryOperatorHandler = new GreaterThanLessThanBinaryOperatorHandler(Left, Right, Operator);
        }

        protected override BinaryOperatorHandler BinaryOperatorHandler { get; }

        public override FilterFunction Operator => FilterFunction.le;
    }
}
