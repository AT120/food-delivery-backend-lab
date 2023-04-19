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
    DishSortingFuncs = new()
    {
        {SortingTypes.NameAsc, x => x.OrderBy(y => y.Name)},
        {SortingTypes.NameDesc, x => x.OrderByDescending(y => y.Name)},
        {SortingTypes.PriceAsc, x => x.OrderBy(y => y.Price)},
        {SortingTypes.PriceDesc, x => x.OrderByDescending(y => y.Price)},
        {SortingTypes.RatingAsc, x => x.OrderBy(y => y.Rating)},
        {SortingTypes.RatingDesc, x => x.OrderByDescending(y => y.Rating)},
    };

    public static readonly Dictionary
    <
        StaffOrderSortingTypes,
        Func<IQueryable<Order>, IOrderedQueryable<Order>>
    > 
    StaffSortingFuncs = new()
    {
        {StaffOrderSortingTypes.CreationTimeAsc, x => x.OrderBy(y => y.OrderTime)},
        {StaffOrderSortingTypes.CreationTimeDesc, x => x.OrderByDescending(y => y.OrderTime)},
        {StaffOrderSortingTypes.DeliveryTimeAsc, x => x.OrderBy(y => y.DeliveryTime)},
        {StaffOrderSortingTypes.DeliveryTimeDesc, x => x.OrderByDescending(y => y.DeliveryTime)},
    };
}