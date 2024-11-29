using Microsoft.AspNetCore.Mvc;
using ShortTermStayAPI.Model.DTOs;
using ShortTermStayAPI.Model.Entities;
using ShortTermStayAPI.Services;
using ShortTermStayAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ShortTermStayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly AuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(AuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDTO request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("User with this email already exists.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _authService.CreatePasswordHash(request.Password),
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateJwtToken(user);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest("User not found.");

            if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
                return BadRequest("Wrong password.");

            var token = _authService.GenerateJwtToken(user);
            return Ok(token);
        }
    }
}