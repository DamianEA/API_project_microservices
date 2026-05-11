using Microsoft.AspNetCore.Mvc;
using Drive.Models;
using Microsoft.EntityFrameworkCore;

namespace Drive.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DefaultDbContext _context;

    public UserController(DefaultDbContext context)
    {
        _context = context;
    }
    
    [EndpointSummary("Regresa la lista de usuarios")]
    [HttpGet]
    public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10)
    {        
        List<User> users = await _context.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {        
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.id == id);

        if(user == null)
            return NotFound();

        return Ok(user);
    }

//////////////////////////////////////////////////////////////////////////////////////////

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUser userData)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newUser = new User
        {
            name = userData.name ?? string.Empty,
            email = userData.email ?? string.Empty,
            pass = Drive.Models.User.GetHash(userData.pass ?? string.Empty), 
            birth = DateTime.SpecifyKind(userData.birth, DateTimeKind.Utc),
            roll = userData.roll 
        };

        try 
        {

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();  
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.id }, newUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
    {
        string hashedPass = Drive.Models.User.GetHash(credentials.pass);
        var user = await _context.Users
            .AsNoTracking() 
            .FirstOrDefaultAsync(u => u.email == credentials.email && u.pass == hashedPass);

        if (user == null)
        {
            return Unauthorized(new { message = "Email o contraseña incorrectos" });
        }
        return Ok(new { name = user.name }); 
    }
}

public class UserCredentials
{
    public string email { get; set; } = string.Empty;
    public string pass { get; set; } = string.Empty;
}
