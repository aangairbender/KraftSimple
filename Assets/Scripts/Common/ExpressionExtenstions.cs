using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Assets.Scripts.Common
{
    internal static class ExpressionExtenstions
    {
        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            var lambda = (LambdaExpression)expression;

            var memberExpression = lambda.Body switch
            {
                UnaryExpression unary when unary.Operand is MemberExpression unaryMember => unaryMember,
                MemberExpression member => member,
                _ => throw new ArgumentException("Not a property expression", nameof(expression)),
            };

            return memberExpression.Member;
        }
    }
}
