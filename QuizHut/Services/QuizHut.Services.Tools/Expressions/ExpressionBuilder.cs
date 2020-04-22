namespace QuizHut.Services.Tools.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using QuizHut.Data.Common.Enumerations;

    public class ExpressionBuilder : IExpressionBuilder
    {
        private const string Name = "Name";
        private const string FullName = "FullName";
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string QuizName = "QuizName";
        private const string EventName = "EventName";
        private const string Administrator = "Administrator";
        private const string Teacher = "Teacher";
        private const string StatusEnded = "Ended";
        private const string StatusActive = "Active";
        private const string StatusPending = "Pending";
        private const string StatusString = "Status";
        private const string Assigned = "Assigned";
        private const string Unassigned = "Unassigned";
        private const string Email = "Email";
        private const string EventId = "EventId";
        private const string ParameterName = "x";

        public Expression<Func<T, bool>> GetExpression<T>(string queryType, string queryValue, string roleId = null)
        {
            var parameter = Expression.Parameter(typeof(T), ParameterName);
            switch (queryType)
            {
                case Name:
                case FirstName:
                case LastName:
                case Email:
                case EventName:
                case QuizName:
                    var expressionBody = this.GetContainsMethod<T>(queryType, queryValue, parameter);
                    return Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
                case StatusEnded:
                case StatusActive:
                case StatusPending:
                    return this.GetEqualMethod<T>(queryType, queryValue, parameter, true);
                case Assigned:
                    return this.GetEqualMethod<T>(queryType, queryValue, parameter, false);
                case Unassigned:
                    return this.GetEqualMethod<T>(queryType, queryValue, parameter, true);
                case FullName:
                case Administrator:
                case Teacher:
                    return this.GetFullNameConcatMethod<T>(queryValue, parameter);
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

        private Expression<Func<T, bool>> GetEqualMethod<T>(string queryType, string queryValue, ParameterExpression parameter, bool equality)
        {
            var nameOfProperty = this.GetParameterName(queryType);
            Expression property = Expression.Property(parameter, nameOfProperty);
            Expression right = Expression.Constant(this.GetType(queryType));
            var call = equality == true ? Expression.Equal(property, right) : Expression.NotEqual(property, right);
            if (queryValue != null)
            {
                call = Expression.AndAlso(call, this.GetContainsMethod<T>(Name, queryValue, parameter));
            }

            return Expression.Lambda<Func<T, bool>>(call, parameter);
        }

        private Expression<Func<T, bool>> GetFullNameConcatMethod<T>(string queryValue, ParameterExpression parameter)
        {
            var firstNameProperty = Expression.PropertyOrField(parameter, FirstName);
            var lastNameProperty = Expression.PropertyOrField(parameter, LastName);

            var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[0]);
            var concatMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            var conntaisMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

            var firstNameToLowerCall = Expression.Call(firstNameProperty, toLowerMethod);
            var lastNameToLowerCall = Expression.Call(lastNameProperty, toLowerMethod);

            var concatCall = Expression.Call(concatMethod, firstNameToLowerCall, lastNameToLowerCall);
            queryValue = string.Join(string.Empty, queryValue.Split(new[] { ' ', ',', '.', ':', '=', ';' }, StringSplitOptions.RemoveEmptyEntries)).ToLower();
            var constant = Expression.Constant(queryValue, typeof(string));
            var expressionBody = Expression.Call(concatCall, conntaisMethod, constant);
            return Expression.Lambda<Func<T, bool>>(expressionBody, parameter);
        }

        private object GetType(string queryType)
        {
            return queryType switch
            {
                StatusActive => Status.Active,
                StatusEnded => Status.Ended,
                StatusPending => Status.Pending,
                Unassigned => null,
                Assigned => null,
                _ => throw new InvalidFilterCriteriaException(),
            };
        }

        private string GetParameterName(string queryType)
        {
            switch (queryType)
            {
                case StatusActive:
                case StatusPending:
                case StatusEnded: return StatusString;
                case Name:
                case FirstName:
                case LastName:
                case EventName:
                case QuizName:
                case Email: return queryType;
                case Assigned:
                case Unassigned: return EventId;
                default: throw new InvalidFilterCriteriaException();
            }
        }
    }
}
