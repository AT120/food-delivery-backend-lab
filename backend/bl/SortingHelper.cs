using System.Buffers;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using BackendCommon.Enums;
using BackendDAL.Entities;

namespace BackendBl;


public static class SortingHelper
{    
    public static readonly Dictionary
    <
        SortingTypes,
        Func<IQueryable<Dish>, IOrderedQueryable<Dish>>
    > 
    SortingFuncs = new()
    {
        {SortingTypes.NameAsc, x => x.OrderBy(y => y.Name)},
        {SortingTypes.NameDesc, x => x.OrderByDescending(y => y.Name)},
        {SortingTypes.PriceAsc, x => x.OrderBy(y => y.Price)},
        {SortingTypes.PriceDesc, x => x.OrderByDescending(y => y.Price)},
        {SortingTypes.RatingAsc, x => x.OrderBy(y => y.Rating)},
        {SortingTypes.RatingDesc, x => x.OrderByDescending(y => y.Rating)},
    };

}