using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeAuthAPI.Data;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    public IActionResult GetAllUsers()
    {
        var currentUsername = User.Identity?.Name;
        var currentRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        var users = _context.Users.AsQueryable();

        switch (currentRole)
        {
            case "SuperAdmin":
                users = users.Where(u =>
                    u.Username == currentUsername ||        // their own
                    u.Role == "Admin" ||
                    u.Role == "Employee");
                break;

            case "Admin":
                users = users.Where(u =>
                    u.Username == currentUsername ||        // their own
                    u.Role == "Employee");
                break;

            case "Employee":
                users = users.Where(u => u.Username == currentUsername); // only their own
                break;

            default:
                return Forbid(); // 403
        }

        return Ok(users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Role
        }).ToList());
    }

}
