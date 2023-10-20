using ABGAssignment.ApplicationContext;
using ABGAssignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ABGAssignment.Controllers
{
    //[Route("[controller]")]
    //[ApiController]
    public class UserController : Controller
    {
        private readonly AbgDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration, AbgDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList(); 

            if (users == null)
            {
                return NotFound(); 
            }

            return Ok(users); 
        }



        [HttpPost ("CreateUser")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Securely hash the password here
            user.Password = HashPassword(user.Password);

            // Call the stored procedure to insert user data
            var nameParam = new SqlParameter("@Name", user.Name);
            var emailParam = new SqlParameter("@Email", user.EmailId);
            var mobileParam = new SqlParameter("@MobileNumber", user.MobileNumber);
            var userIdParam = new SqlParameter("@UserId", user.UserId);
            var passwordParam = new SqlParameter("@PasswordHash", user.Password);

            try
            {
                var insertResult = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC InsertUser @Name, @Email, @MobileNumber, @UserId, @PasswordHash",
                    nameParam, emailParam, mobileParam, userIdParam, passwordParam);

                if (insertResult > 0)
                {
                    return CreatedAtAction("GetUser", new { id = user.Id }, user);
                }
                else
                {
                    return BadRequest("User insertion failed.");
                }
            }
            catch (DbUpdateException ex)
            {
                // Handle database exceptions here if needed
                return BadRequest("User insertion failed. " + ex.Message);
            }
        }

       
        private string HashPassword(string password)
        {
            // Generate a salt and hash the password using BCrypt
            string salt = BCrypt.Net.BCrypt.GenerateSalt(15); 
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }


        // Login with JWT Token

        [HttpPost("Login")]
        public async Task<IActionResult> Login()
        {
            // Read and parse the request body to get the login credentials
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            var loginRequest = JsonConvert.DeserializeObject<UserLogin>(requestBody);

            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.UserId) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Invalid request data.");
            }

            // Check the user's credentials (e.g., compare with the stored hashed password)
            var user = _context.Users.SingleOrDefault(u => u.UserId == loginRequest.UserId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return Unauthorized();
            }

            // Create a security token and a handler for JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserId),
            new Claim(ClaimTypes.Email, user.EmailId),
        }),
                /* Expires = DateTime.UtcNow.AddHours(1), */// Token expiration time
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }


    }
}
