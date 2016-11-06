using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Utilities
{
    public static class ExpressionExtensions
    {
        public static PropertyPath GetSimplePropertyAccess(this LambdaExpression propertyAccessExpression)
        {
            var propertyPath = propertyAccessExpression.Parameters.Single().MatchSimplePropertyAccess(propertyAccessExpression.Body);

            if (propertyPath == null)
                throw new InvalidOperationException("InvalidPropertyExpression");

            return propertyPath;
        }

        public static PropertyPath GetComplexPropertyAccess(this LambdaExpression propertyAccessExpression)
        {
            var propertyPath = propertyAccessExpression.Parameters.Single().MatchComplexPropertyAccess(propertyAccessExpression.Body);

            if (propertyPath == null)
                throw new InvalidOperationException("InvalidComplexPropertyExpression");

            return propertyPath;
        }

        public static IEnumerable<PropertyPath> GetSimplePropertyAccessList(this LambdaExpression propertyAccessExpression)
        {
            var propertyPaths = MatchPropertyAccessList(propertyAccessExpression, (p, e) => e.MatchSimplePropertyAccess(p));

            if (propertyPaths == null)
                throw new InvalidOperationException("InvalidPropertyExpression");

            return propertyPaths;
        }

        public static IEnumerable<PropertyPath> GetComplexPropertyAccessList(this LambdaExpression propertyAccessExpression)
        {
            var propertyPaths = MatchPropertyAccessList(propertyAccessExpression, (p, e) => e.MatchComplexPropertyAccess(p));

            if (propertyPaths == null)
                throw new InvalidOperationException("InvalidComplexPropertyExpression");

            return propertyPaths;
        }

        private static IEnumerable<PropertyPath> MatchPropertyAccessList(this LambdaExpression lambdaExpression, Func<Expression, Expression, PropertyPath> propertyMatcher)
        {
            var newExpression = RemoveConvert(lambdaExpression.Body) as NewExpression;

            if (newExpression != null)
            {
                var parameterExpression = lambdaExpression.Parameters.Single();

                var propertyPaths = newExpression.Arguments.Select(a => propertyMatcher(a, parameterExpression)).Where(p => p != null);

                if (propertyPaths.Count() == newExpression.Arguments.Count())
                    return newExpression.HasDefaultMembersOnly(propertyPaths) ? propertyPaths : null;
            }

            var propertyPath = propertyMatcher(lambdaExpression.Body, lambdaExpression.Parameters.Single());

            return (propertyPath != null) ? new[] { propertyPath } : null;
        }

        private static bool HasDefaultMembersOnly(this NewExpression newExpression, IEnumerable<PropertyPath> propertyPaths)
        {
            return !newExpression.Members.Where((t, i) => !string.Equals(t.Name, propertyPaths.ElementAt(i).Last().Name, StringComparison.Ordinal)).Any();
        }

        private static PropertyPath MatchSimplePropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyPath = MatchPropertyAccess(parameterExpression, propertyAccessExpression);

            return propertyPath != null && propertyPath.Count == 1 ? propertyPath : null;
        }

        private static PropertyPath MatchComplexPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            return MatchPropertyAccess(parameterExpression, propertyAccessExpression);
        }

        private static PropertyPath MatchPropertyAccess(this Expression parameterExpression, Expression propertyAccessExpression)
        {
            var propertyInfos = new List<PropertyInfo>();

            MemberExpression memberExpression;

            do
            {
                memberExpression = RemoveConvert(propertyAccessExpression) as MemberExpression;

                if (memberExpression == null)
                    return null;

                var propertyInfo = memberExpression.Member as PropertyInfo;

                if (propertyInfo == null)
                    return null;

                propertyInfos.Insert(0, propertyInfo);
                propertyAccessExpression = memberExpression.Expression;
            }
            while (memberExpression.Expression != parameterExpression);

            return new PropertyPath(propertyInfos);
        }

        public static Expression RemoveConvert(this Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        public static bool IsNullConstant(this Expression expression)
        {
            // convert statements introduced by compiler should not affect nullness
            expression = expression.RemoveConvert();

            // check if the unwrapped expression is a null constant
            if (expression.NodeType != ExpressionType.Constant)
                return false;

            return ((ConstantExpression)expression).Value == null;
        }

        public static bool IsStringAddExpression(this Expression expression)
        {
            var linq = expression as BinaryExpression;
            if (linq == null)
                return false;

            if (linq.Method == null || linq.NodeType != ExpressionType.Add)
                return false;

            return linq.Method.DeclaringType == typeof(string) && string.Equals(linq.Method.Name, "Concat", StringComparison.Ordinal);
        }
    }
}
