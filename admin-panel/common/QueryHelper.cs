using System.ComponentModel;
using AdminCommon.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using ProjCommon.DTO;

namespace AdminCommon;

public static class QueryHelper
{
    public static T? TryParseSingle<T>(this IQueryCollection queris, string key)
    {
        var sqvalues = queris[key];
        if (sqvalues == StringValues.Empty)
            return default;

        try
        {
            
            return (T?)Convert.ChangeType(sqvalues.First(), typeof(T));
        }
        catch
        {
            return default;
        }
        
    }

    public static T? TryParseSingle<T>(
        this IQueryCollection queris,
        string key,
        Func<string, T> converter)
    {
        var sqvalues = queris[key];
        if (sqvalues == StringValues.Empty)
            return default;

        try
        {    
            return converter(sqvalues.First()!);
        }
        catch
        {
            return default;
        }
        
    }

    private static T DefaultConverter<T>(object value) 
        => (T)Convert.ChangeType(value, typeof(T));
        

    public static IEnumerable<T>? TryParseMany<T>(this IQueryCollection queris, string key)
    {
        var sqvalues = queris[key];
        if (sqvalues == StringValues.Empty)
            return default;

        var res = new List<T>(sqvalues.Count);
        try
        {
            foreach (var value in sqvalues)
                res.Add(DefaultConverter<T>(value));
            
            return res;
        }
        catch
        {
            return default;
        }
    }

    public static IEnumerable<T>? TryParseMany<T>(
        this IQueryCollection queris,
        string key,
        Func<string, T> converter)
    {
        var sqvalues = queris[key];
        if (sqvalues == StringValues.Empty)
            return default;

        var res = new List<T>(sqvalues.Count);
        try
        {
            foreach (var value in sqvalues)
                res.Add(converter(value));
            
            return res;
        }
        catch
        {
            return default;
        }
    }



    public static string GetSearchQuery(this HttpContext context)
    {
        var sqvalues = context.Request.Query["searchQuery"];
        return (sqvalues == StringValues.Empty) ? "" : sqvalues[0]!;
    }
}