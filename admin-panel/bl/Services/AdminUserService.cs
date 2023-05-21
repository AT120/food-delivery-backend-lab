using AdminCommon.DTO;
using AdminCommon.Interfaces;
using AuthDAL;
using AuthDAL.Models;
using BackendDAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjCommon;
using ProjCommon.Const;
using ProjCommon.DTO;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace AdminBL.Services;

public class AdminUserService : IAdminUserService
{
    private readonly AuthDBContext _authDBContext;
    private readonly BackendDBContext _backendDBContext;
    private readonly UserManager<User> _userManager;
    public AdminUserService(
        AuthDBContext adbc,
        UserManager<User> um,
        BackendDBContext bdbc)
    {
        _userManager = um;
        _authDBContext = adbc;
        _backendDBContext = bdbc;
    }

    private readonly BackendException BadRestaurantId
        = new(400, "Необходимо указать ресторан для повара или менеджера");



    private async Task AddUserToRole(User user, RoleType role, Guid? restaurantId)
    {
        var res = await _userManager.AddToRoleAsync(user, role.ToString());
        if (!res.Succeeded)
            throw new BackendException(400, res.Errors.First().Description);


        switch (role)
        {
            case RoleType.Cook:
                await _authDBContext.Cooks.AddAsync(new Cook
                {
                    BaseUser = user
                });
                await _backendDBContext.Cooks.AddAsync(new BackendDAL.Entities.Cook
                {
                    Id = user.Id,
                    RestaurantId = restaurantId ?? throw BadRestaurantId
                });
                break;

            case RoleType.Customer:
                await _authDBContext.Customers.AddAsync(new Customer
                {
                    BaseUser = user
                });
                break;

            case RoleType.Courier:
                await _authDBContext.Couriers.AddAsync(new Courier
                {
                    BaseUser = user
                });
                await _backendDBContext.Couriers.AddAsync(new BackendDAL.Entities.Courier
                {
                    Id = user.Id,
                });
                break;

            case RoleType.Manager:
                await _authDBContext.Managers.AddAsync(new Manager
                {
                    BaseUser = user
                });
                await _backendDBContext.Managers.AddAsync(new BackendDAL.Entities.Manager
                {
                    Id = user.Id,
                    RestaurantId = restaurantId ?? throw BadRestaurantId
                });
                break;
        }
    }

    private async Task RemoveUserFromRole(User user, RoleType role)
    {
        var res = await _userManager.RemoveFromRoleAsync(user, role.ToString());
        if (!res.Succeeded)
            throw new BackendException(400, res.Errors.First().Description);

        switch (role)
        {
            case RoleType.Cook:
                _authDBContext.Cooks.Remove(new Cook { BaseUserId = user.Id });
                _backendDBContext.Cooks.Remove(new BackendDAL.Entities.Cook { Id = user.Id });
                break;

            case RoleType.Customer:
                _authDBContext.Customers.Remove(new Customer { BaseUserId = user.Id });
                break;

            case RoleType.Courier:
                _authDBContext.Couriers.Remove(new Courier { BaseUserId = user.Id });
                _backendDBContext.Couriers.Remove(new BackendDAL.Entities.Courier { Id = user.Id });
                break;

            case RoleType.Manager:
                _authDBContext.Managers.Remove(new Manager { BaseUserId = user.Id });
                _backendDBContext.Managers.Remove(new BackendDAL.Entities.Manager { Id = user.Id });
                break;
        }
    }

    private void UpdateUserRestaurant(
        User user,
        Guid? restaurantId,
        IEnumerable<RoleType> userRoles)
    {
        bool isCook = userRoles.Any(r => r == RoleType.Cook);
        bool isManager = userRoles.Any(r => r == RoleType.Manager);

        if (isCook)
        {
            _backendDBContext.Cooks.Update(new BackendDAL.Entities.Cook
            {
                Id = user.Id,
                RestaurantId = restaurantId ?? throw BadRestaurantId
            });
        }
        if (isManager)
        {
            _backendDBContext.Managers.Update(new BackendDAL.Entities.Manager
            {
                Id = user.Id,
                RestaurantId = restaurantId ?? throw BadRestaurantId
            });
        }
    }

    private async Task<bool> CheckRestaurant(IEnumerable<RoleType> roles, Guid? restaurnatId)
    {
        if (roles.Any(r => r == RoleType.Cook || r == RoleType.Manager))
        {
            if (restaurnatId == null)
                return false;

            bool restaurantExists = await _backendDBContext.Restaurants
                .AnyAsync(r => r.Id == restaurnatId);
                // throw new BackendException(404, "Указанный ресторан не существует");
            return restaurantExists;
        }

        return true;
    }

    private async Task<User> GetUserWithRoles(Guid userId)
    {
        return await _userManager.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new BackendException(404, "Такого пользователя не существует");
    }

    public async Task<Page<UserProfile>> GetUsers(
        int page,
        string? nameSearchQuery = null,
        string? emailSearchQuery = null,
        string? phoneSearchQuery = null,
        Gender? gender = null,
        IEnumerable<RoleType>? roles = null)
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
            query = query.Where(u =>
                u.Roles.Any(userRole =>
                    roles!.Contains(userRole.RoleType)
                )
            );
        }

        int size = await query.CountAsync();
        PageInfo pageInfo = new(page, size, PageSize.AdminPanel);

        if (pageInfo.RangeStart > size)
            throw new BackendException("переработать");  //TODO:

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


    public async Task<UserProfileDetailed> GetUser(Guid userId)
    {
        var user = await GetUserWithRoles(userId);

        bool isCook = user.Roles.Any(u => u.RoleType == RoleType.Cook);
        bool isManager = user.Roles.Any(u => u.RoleType == RoleType.Manager);

        // var restaurants = await _backendDBContext.Restaurants
        //     .Select(r => new AvailableRestaurant
        //     {
        //         Id = r.Id,
        //         Name = r.Name
        //     }).ToListAsync();

        if (isCook)
        {
            var cook = await _backendDBContext.Cooks
                .Include(u => u.Restaurant)
                .FirstOrDefaultAsync(u => u.Id == userId)
                    ?? throw new BackendException(500, "Ужасные вещи случились.");
            
            return Converter.GetUserProfileDetailed(user, cook.Restaurant);
        }
        else if (isManager)
        {
            var manager = await _backendDBContext.Managers
                .Include(u => u.Restaurant)
                .FirstOrDefaultAsync(u => u.Id == userId)
                    ?? throw new BackendException(500, "Ужасные вещи случились.");
            
            return Converter.GetUserProfileDetailed(user, manager.Restaurant);
        }
        else 
        {
            return Converter.GetUserProfileDetailed(user);
        }
    }


    public async Task EditUser(UserProfileEdit newUser)
    {
        var user = await GetUserWithRoles(newUser.UserId);


        if (!await CheckRestaurant(newUser.Roles, newUser.NewRestaurantId))
            throw new  BackendException(404, "Указанный ресторан не существует");

        var oldRoles = user.Roles.Select(r => r.RoleType).ToList();
        var rolesToAdd = newUser.Roles.Except(oldRoles);
        var rolesToDelete = oldRoles.Except(newUser.Roles);
        var oldUnchangedRoles = oldRoles.Intersect(newUser.Roles);
        foreach (var role in rolesToAdd)
        {
            await AddUserToRole(user, role, newUser.NewRestaurantId);
        }

        foreach (var role in rolesToDelete)
        {
            await RemoveUserFromRole(user, role);
        }

        UpdateUserRestaurant(user, newUser.NewRestaurantId, oldUnchangedRoles);

        //TODO: revert on error
        // Task.WaitAll(new Task[] {
            await _authDBContext.SaveChangesAsync();
            await _backendDBContext.SaveChangesAsync();
        // });
    }

    public async Task CreateUser(UserProfileCreate profile)
    {
        var user = new User 
        {
            UserName = profile.UserData.Email,
            Email = profile.UserData.Email,
            FullName = profile.UserData.Name,
            PhoneNumber = profile.UserData.PhoneNumber,
            Gender = profile.UserData.Gender,
            BirthDate = profile.UserData.BirthDate,
        };

        var res = await _userManager.CreateAsync(user, profile.UserData.Password);
        if (!res.Succeeded)
            throw new BackendException(400, res.Errors.First().Description);
        
        if (!await CheckRestaurant(profile.Roles, profile.ResturantId))
            throw new  BackendException(404, "Указанный ресторан не существует");
        
        foreach (RoleType role in profile.Roles)
            await AddUserToRole(user, role, profile.ResturantId);

        Task.WaitAll(new Task[] {
            _authDBContext.SaveChangesAsync(),
            _backendDBContext.SaveChangesAsync()
        });
    }


    public async Task DeleteUser(Guid userId)
    {
        var user = await GetUserWithRoles(userId);
        var roles = user.Roles.Select(r => r.RoleType).ToList();
        foreach (RoleType role in roles) 
        {
            await RemoveUserFromRole(user, role);
        }
        
        await _userManager.DeleteAsync(user);

        Task.WaitAll(new Task[] {
            _authDBContext.SaveChangesAsync(),
            _backendDBContext.SaveChangesAsync()
        });
    }
}