using System.Linq.Expressions;
using System.Reflection;

namespace ProjCommon.Helpers;

public static class ExpressionHelper //TODO: refactor
{
    public static Expression<Func<TPropertyBase, bool>> 
        GetOrExpression<T, TPropertyBase>(IEnumerable<T> possibleOptions, PropertyInfo property)
    {
        var arg = Expression.Parameter(typeof(TPropertyBase));

        var expr = Expression.Equal(
            Expression.Constant(possibleOptions.First()),
            Expression.MakeMemberAccess(arg, property)
        );

        foreach (var option in possibleOptions.Skip(1))
        {
            var disjunction = Expression.Equal(
                Expression.Constant(option),
                Expression.MakeMemberAccess(arg, property)
            );
            expr = Expression.OrElse(expr, disjunction);
        }

        return Expression.Lambda<Func<TPropertyBase, bool>>(expr, arg);
    }    
}