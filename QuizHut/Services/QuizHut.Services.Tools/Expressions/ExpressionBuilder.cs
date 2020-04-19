namespace QuizHut.Services.Tools.Expressions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using QuizHut.Data.Common.Enumerations;

    public class ExpressionBuilder : IExpressionBuilder
    {
        private const string Name = "Name";
        private const string StatusEnded = "Ended";
        private const string StatusActive = "Active";
        private const string StatusString = "Status";
        private const string Email = "Email";
        private const string ParameterName = "x";




        public Expression<Func<T, bool>> GetExpression<T>(string queryType, string queryValue)
        {
            var parameter = Expression.Parameter(typeof(T), ParameterName);
            MethodCallExpression expressionBody;

            switch (queryType)
            {
                case Name:
                    expressionBody = this.GetContainsMethod<T>(queryType, queryValue, parameter);
                    return Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
                case StatusEnded:
                case StatusActive:
                   return  this.GetEqualMethod<T>(queryType, queryValue, parameter);
                default:
                    return null;
            }
        }

        private MethodCallExpression GetContainsMethod<T>(string queryType, string queryValue, ParameterExpression parameter)
        {
            var nameOfProperty = this.GetParameterName(queryType);
            var property = Expression.PropertyOrField(parameter, nameOfProperty);
            MethodInfo method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[0]);
            var call = Expression.Call(property, toLowerMethod);
            var constant = Expression.Constant(queryValue.ToLower(), typeof(string));
            return Expression.Call(call, method, constant); 
        }

        private Expression<Func<T, bool>> GetEqualMethod<T>(string queryType, string queryValue, ParameterExpression parameter)
        {

            var nameOfProperty = this.GetParameterName(queryType);
            Expression property = Expression.Property(parameter, nameOfProperty);  
            Expression right = Expression.Constant(queryType == StatusActive ? Status.Active : Status.Ended);
            var call = Expression.Equal(property, right);
            if (queryValue != null)
            {
                call = Expression.AndAlso(call, this.GetContainsMethod<T>(Name, queryValue, parameter));
            }
            
            return Expression.Lambda<Func<T, bool>>(call, parameter);

        }

        private string GetParameterName(string queryType)
        {
            switch (queryType)
            {
                case Name: return queryType;
                case StatusActive: 
                case StatusEnded: return StatusString;
                case Email: return queryType;
                //case "creation": return "CreatedOn";
                //case "Date": return "ActivationDateAndTime";
                default: throw new InvalidFilterCriteriaException();
            }
        }
    }
}
