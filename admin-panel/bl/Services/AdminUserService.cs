using AdminCommon.Interfaces;
using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Enums;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

namespace AdminBL.Services;

public class AdminUserService : IAdminUserService
{
    private readonly AuthDBContext _authDBContext;  
    private readonly UserManager<User> _userManager;  
    private readonly RoleManager<Role> _roleManager;  
    public AdminUserService(AuthDBContext adbc, UserManager<User> um)
    {
        _userManager = um;
        _authDBContext = adbc;
    }

    public async Task<Page<UserProfile>> GetUsers(
        int page,
        string? nameSearchQuery = null,
        string? emailSearchQuery = null,
        string? phoneSearchQuery = null,
        Gender? gender = null,
        IEnumerable<RoleType>? roles= null)
    {
        if (page < 1)
            throw new BackendException(400, "Incorrect page number");

        var query = _userManager.Users
            .Include(u => u.Roles)
            .AsQueryable();
        
        if (emailSearchQuery != null)
            query = query.Where(u => u.Email.Contains(emailSearchQuery));

        if (nameSearchQuery != null)
            query = query.Where(u => u.FullName.Contains(nameSearchQuery));

        if (phoneSearchQuery != null)
            query = query.Where(u => u.PhoneNumber.Contains(phoneSearchQuery)); //TODO: телефон мож быть null
        
        if (gender != null)
            query = query.Where(u => u.Gender == gender);

        if (!roles.IsNullOrEmpty())
        {
            var filter = ExpressionHelper.GetOrExpression<RoleType, User>(
                roles,
                typeof(User).GetProperty(nameof(User.Roles))
            );
            query = query.Where(filter);
        }

        int size = await query.CountAsync();
        PageInfo pageInfo = new(page, size, PageSize.AdminPanel);
        
        if (pageInfo.RangeStart > size)
            throw new BackendException("переработать");

        var users = await query
            .OrderBy(u => u.FullName) //TODO: добавить сортировку
            .Select(u => Converter.GetUserProfile(u))
            .Skip(pageInfo.RangeStart)
            .Take(PageSize.AdminPanel)
            .ToListAsync();
        
        return new Page<UserProfile>
        {
            PageInfo = pageInfo,
            Items = users
        };
    }


}