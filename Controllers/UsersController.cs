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
        // 1. Validar que el modelo sea correcto (DataAnnotations)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 2. Mapear del DTO (CreateUser) al Modelo de BD (User)
        // Usamos minúsculas porque así están en tu clase User.cs
        var newUser = new User
        {
            name = userData.name ?? string.Empty,
            email = userData.Email ?? string.Empty,
            // Hasheamos la contraseña usando el método estático que tienes en User.cs
            pass = Drive.Models.User.GetHash(userData.pass ?? string.Empty), 
            birth = DateTime.SpecifyKind(userData.birth, DateTimeKind.Utc),
            roll = "User" // O el valor por defecto que prefieras
        };

        try 
        {
            // 3. Guardar en la base de datos
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            // 4. Retornar un 201 Created
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.id }, newUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }
}
