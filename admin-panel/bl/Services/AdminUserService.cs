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
    private readonly RoleManager<Role> _roleManager;
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
                    roles.Contains(userRole.RoleType)
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
        var user = await _userManager.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new BackendException(404, "User does not exist");

        bool isCook = user.Roles.Any(u => u.RoleType == RoleType.Cook);
        bool isManager = user.Roles.Any(u => u.RoleType == RoleType.Manager);

        var restaurants = await _backendDBContext.Restaurants
            .Select(r => new AvailableRestaurant
            {
                RestaurantId = r.Id,
                RestaurantName = r.Name
            }).ToListAsync();

        if (!(isCook || isManager))
            return Converter.GetUserProfileDetailed(user, restaurants);


        Guid? restaurantId = null;
        if (isCook)
        {
            var cook = await _backendDBContext.Cooks.FindAsync(userId);
            restaurantId = cook?.RestaurantId;
        }
        else if (isManager)
        {
            var manager = await _backendDBContext.Managers.FindAsync(userId);
            restaurantId = manager?.RestaurantId;
        }

        if (restaurantId is not null)
        {
            var workPlace = restaurants.FirstOrDefault(r => r.RestaurantId == restaurantId);
            if (workPlace is not null)
                workPlace.UserWorkingHere = true;
        }

        return Converter.GetUserProfileDetailed(user, restaurants);
    }


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
                await _backendDBContext.Couriers.AddAsync(new BackendDAL.Entities.Courier());
                break;

            case RoleType.Manager:
                await _authDBContext.Managers.AddAsync(new Manager
                {
                    BaseUser = user
                });
                await _backendDBContext.Managers.AddAsync(new BackendDAL.Entities.Manager
                {
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
                _authDBContext.Cooks.Remove(new Cook { Id = user.Id });
                _backendDBContext.Cooks.Remove(new BackendDAL.Entities.Cook { Id = user.Id });
                break;

            case RoleType.Customer:
                _authDBContext.Customers.Remove(new Customer { Id = user.Id });
                break;

            case RoleType.Courier:
                _authDBContext.Couriers.Remove(new Courier { Id = user.Id });
                _backendDBContext.Couriers.Remove(new BackendDAL.Entities.Courier { Id = user.Id });
                break;

            case RoleType.Manager:
                _authDBContext.Managers.Remove(new Manager { Id = user.Id });
                _backendDBContext.Managers.Remove(new BackendDAL.Entities.Manager { Id = user.Id });
                break;
        }
    }

    private void ChangeUserRestaurant(
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
        else if (isManager)
        {
            _backendDBContext.Managers.Update(new BackendDAL.Entities.Manager
            {
                Id = user.Id,
                RestaurantId = restaurantId ?? throw BadRestaurantId
            });
        }
    }

    public async Task EditUser(UserProfileEdit newUser)
    {
        var user = await _userManager.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == newUser.UserId)
                ?? throw new BackendException(404, "Такого пользователя не существует");

        if (newUser.Roles.Any(r => r == RoleType.Cook || r == RoleType.Manager))
        {
            if (newUser.NewRestaurantId == null)
                throw BadRestaurantId;
            bool restaurantExists = await _backendDBContext.Restaurants
                .AnyAsync(r => r.Id == newUser.NewRestaurantId);
            if (!restaurantExists)
                throw new BackendException(404, "Указанный ресторан не существует");
        }

        var rolesToAdd = newUser.Roles
            .Except(
                user.Roles.Select(r => r.RoleType)
            );

        var rolesToDelete = user.Roles
            .Select(r => r.RoleType)
            .Except(newUser.Roles);

        foreach (var role in rolesToAdd)
        {
            await AddUserToRole(user, role, newUser.NewRestaurantId);
        }

        foreach (var role in rolesToDelete)
        {
            await RemoveUserFromRole(user, role);
        }

        ChangeUserRestaurant(user, newUser.NewRestaurantId, newUser.Roles);

        //TODO: revert on error
        Task.WaitAll(new Task[] {
            _authDBContext.SaveChangesAsync(),
            _backendDBContext.SaveChangesAsync()
        });
    }
}