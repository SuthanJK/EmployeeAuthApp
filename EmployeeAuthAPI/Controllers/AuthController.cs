using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeAuthAPI.Models;
using EmployeeAuthAPI.Data;
using EmployeeAuthAPI.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string Username,string Password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }
}
