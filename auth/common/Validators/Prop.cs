using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace AuthCommon.Validators;

public static class Property
{
    public static PropertyInfo Get<T>(Expression<Func<T, object?>> selector) 
    {
        var exp = selector.Body;
        if (exp.NodeType != ExpressionType.MemberAccess)
            throw new InvalidExpressionException("Member access expression was expected");
        return  (PropertyInfo) ((MemberExpression)exp).Member;
    }
}