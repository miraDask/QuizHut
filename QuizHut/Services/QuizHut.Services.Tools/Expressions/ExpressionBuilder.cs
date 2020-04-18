namespace QuizHut.Services.Tools.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ExpressionBuilder : IExpressionBuilder
    {
        public Expression<Func<T, bool>> GetExpression<T>(string queryType, string queryValue)
        {
            var nameOfProperty = this.GetParameterName(queryType);
            MethodInfo method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[0]);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, nameOfProperty);
            var x = Expression.Call(property, toLowerMethod);
            var expressionBody = Expression.Call(x, method, Expression.Constant(queryValue.ToLower(), typeof(string)));
            return Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
        }

        private string GetParameterName(string queryType)
        {
            return "Name";
        }
    }
}
