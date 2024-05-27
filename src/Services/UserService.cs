using api.Dtos;
using api.Dtos.User;
using AutoMapper;
using Dtos.Pagination;
using Dtos.User.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDBContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(AppDBContext dbContext, ILogger<UserService> logger, IPasswordHasher<User> passwordHasher, IMapper mapper)
    {

        _passwordHasher = passwordHasher;
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PaginationResult<UserDto>> GetAllUsersAsync(QueryParameters queryParams)
    {
        var query = _dbContext.Users.AsQueryable();
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query = query.Where(u => u.Username.Contains(queryParams.SearchTerm));
        }
        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query = query.Where(u => u.Email.Contains(queryParams.SearchTerm));
        }

        switch (queryParams.SortBy.ToLower())
        {
            case "name":
                query = queryParams.IsAscending ? query.OrderBy(p => p.Username) : query.OrderByDescending(p => p.Username);
                break;
            case "date":
                query = queryParams.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                break;
            default:
                query = query.OrderBy(p => p.CreatedAt);
                break;
        }


        var totalPages = (int)Math.Ceiling((decimal)totalCount / queryParams.PageSize);
        var users = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(u => _mapper.Map<UserDto>(u))
            .ToListAsync();

        return new PaginationResult<UserDto>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
        };
    }

    public async Task<UserProfileDto> GetUserById(Guid userId)
    {
        // return await _dbContext.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.UserID == userId);
        var user = await _dbContext.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.UserID == userId);
        // var user = await _dbContext.Users.FindAsync(userId);
        var userDto = _mapper.Map<UserProfileDto>(user);
        return userDto;
    }

    public async Task<bool> CreateUser(SignupDto newUser)
    {
        // var isExist = _dbContext.Users.FirstOrDefaultAsync(u => u.Username == newUser.Username || u.Email == newUser.Email);
        var isExist = _dbContext.Users.FirstOrDefault(u => u.Username == newUser.Username || u.Email == newUser.Email);
        if (isExist != null)
        {
            return false;
        }
        var createUser = new User
        {
            Username = newUser.Username,
            Email = newUser.Email,
            Password = _passwordHasher.HashPassword(null, newUser.Password),
        };

        _dbContext.Users.Add(createUser);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUser(Guid userId, UserProfileDto updateUser)
    {
        var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (existingUser != null && updateUser != null)
        {
            existingUser.Username = updateUser.Username;
            existingUser.Email = updateUser.Email;
            existingUser.FirstName = updateUser.FirstName;
            existingUser.LastName = updateUser.LastName;
            existingUser.Address = updateUser.Address;
            existingUser.PhoneNumber = updateUser.PhoneNumber;
            // existingUser.IsBanned = updateUser.IsBanned;
            // existingUser.IsAdmin = updateUser.IsAdmin;

            await _dbContext.SaveChangesAsync();
            return true; // Return true indicating successful update
        }

        return false; // Return false if either existingUser or updateUser is null
    }

    public async Task<bool> DeleteUser(Guid userId)
    {
        var userToDelete = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (userToDelete != null)
        {
            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<UserDto?> LoginUserAsync(LoginDto loginDto)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }
        var userDto = new UserDto
        {
            UserID = user.UserID,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            Address = user.Address,
            IsAdmin = user.IsAdmin,
            IsBanned = user.IsBanned
        };

        return userDto;
    }

}
