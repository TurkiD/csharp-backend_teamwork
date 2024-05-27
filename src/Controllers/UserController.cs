using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using api.Controllers;
using api.Dtos.User;
using api.Middlewares;
using api.Services;
using Dtos.User.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuthService _authService;
    private readonly ILogger<UserController> _logger;

    public UserController(UserService userService, ILogger<UserController> logger, AuthService authService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger;
        _authService = authService;
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] QueryParameters queryParameters)
    {
        var users = await _userService.GetAllUsersAsync(queryParameters);
        if (users == null)
        {
            throw new NotFoundException("No user Found");
        }
        return ApiResponse.Success(users, "all users are returned successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    public IActionResult GetUser(Guid userId)
    {
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User Returned");
    }

    // Singed in user only can get the information of their account
    [HttpGet("user/profile")]
    public IActionResult GetUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User Returned");
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] SignupDto newUser)
    {
        var createdUser = await _userService.CreateUser(newUser);
        if (createdUser)
        {
            return ApiResponse.Created("User is created successfully");
        }
        else
        {
            // throw new BadRequestException("User Already exist");
            return ApiResponse.BadRequest("Username or Email Already Exist");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException("Invalid User Data");
        }
        var loggedInUser = await _userService.LoginUserAsync(loginDto);
        if (loggedInUser == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _authService.GenerateJwt(loggedInUser);
        return ApiResponse.Success(new { token, loggedInUser }, "User Logged In successfully");

    }

    [HttpPut("user")]
    public async Task<IActionResult> UpdateUser([FromBody] UserProfileDto updateUser)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = await _userService.UpdateUser(userId, updateUser);
        if (!user)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Updated("User is updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("user/{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _userService.DeleteUser(userId);
        if (!result)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Deleted("User is deleted successfully");
    }

    [HttpDelete("user")]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var result = await _userService.DeleteUser(userId);
        if (!result)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Deleted("User is deleted successfully");
    }
}
