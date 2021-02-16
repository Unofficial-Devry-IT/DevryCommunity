using System.Collections;
using System.Linq.Expressions;

namespace DevryServices.Common.Extensions
{
    using System.Reflection;
    using System;
    
    public static class ReflectionExtensions
    {
        public static PropertyInfo GetProp<T>(this T obj, Expression<Func<T, object>> selector)
        {
            if (selector.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("Selector must be lambda expression", nameof(selector));

            var lambda = (LambdaExpression) selector;
            var memberExpression = ExtractMemberExpression(lambda.Body);

            if (memberExpression == null)
                throw new ArgumentNullException("Selector must be member access expression", nameof(selector));

            if (memberExpression.Member.DeclaringType == null)
                throw new InvalidOperationException("Property does not have declaring type");

            return memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name);
        }

        static MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
                return ((MemberExpression) expression);
            if (expression.NodeType == ExpressionType.Convert)
            {
                var operand = ((UnaryExpression) expression).Operand;
                return ExtractMemberExpression(operand);
            }

            return null;
        }
    }
}