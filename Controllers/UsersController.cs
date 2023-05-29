using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using TrackerApplication.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace TrackerApplication.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller {
        private readonly DataContext _context;
        public UsersController(DataContext context) {
            _context = context;
        }
        // [HttpPost("login")]
        // public IActionResult Login([FromBody] User login) {
        //     var dbUser = _context.Users!.FirstOrDefault(user => user.Username == login.Username);

        //     if (dbUser == null) return NotFound();

        //     if (dbUser.Key != HashPassword(login.Key)) return Unauthorized();

        //     var token = GenerateJSONWebToken(dbUser);

        //     return Ok(new { token = token });
        // }

        [HttpPost]
        public IActionResult PostPut([FromBody] User user) {
            var dbUser = _context.Users!.First();
            if (dbUser.Hash != user.Hash) return BadRequest("Authentication failed");
            if (dbUser.Tenant == user.Tenant.ToLower() && dbUser.Username == user.Username.ToLower()) return BadRequest("Not allowed");
            if (user.Tenant == "" || user.Username == "" || user.Company == "" || user.Key == "") return BadRequest("Empty fields not allowed");

            var query = _context.Users!.AsQueryable().Where(x => x.Tenant == user.Tenant.ToLower());
            query = query.Where(x => x.Username == user.Username.ToLower());
            query = query.Where(x => x.Company == user.Company.ToLower());

            // Post
            dbUser = query.FirstOrDefault();
            if (dbUser == null) {
                user.Tenant = user.Tenant.ToLower();
                user.Username = user.Username.ToLower();
                user.Company = user.Company.ToLower();
                user.Hash = "";

                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok("User Added");
            }

            if (dbUser.Key == user.Key) return Conflict("Key already present");

            // Update
            dbUser.Key = user.Key;
            _context.Update(dbUser);
            _context.SaveChanges();
            return Ok("User Changed");
        }

        // private string HashPassword(string password) {
        //     string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //         password: password,
        //         salt: new byte[0],
        //         prf: KeyDerivationPrf.HMACSHA256,
        //         iterationCount: 100000,
        //         numBytesRequested: 256 / 8));

        //     return hashed;
        // }

        // private string GenerateJSONWebToken(User user) {
        //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //     var token = new JwtSecurityToken(_config["Jwt:Issuer"],
        //       _config["Jwt:Issuer"],
        //       //   new List<Claim> { new Claim("organizationId", user.OrganizationId.ToString()),
        //       //   new Claim(ClaimTypes.Role, user.Roles)},
        //       expires: DateTime.Now.AddMinutes(10),
        //       signingCredentials: credentials);

        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }
    }
}
