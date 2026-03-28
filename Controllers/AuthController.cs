using Microsoft.AspNetCore.Mvc;
using Drive.Models;
using System.Security.Claims;

namespace Drive.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DefaultDbContext _context;

    public AuthController(DefaultDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserCredentials userCredentials)
    {
        if(ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == userCredentials.Email);

            if(user != null && Models.User.GetHash(userCredentials.pass) != user.pass)
                return Ok();
        }

        return Unauthorized();
    }
}