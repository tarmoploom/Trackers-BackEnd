using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TrackerApplication.Model;
using System.Net.Http.Headers;
using System.Text;

namespace TrackerApplication.Controllers {

    //[Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : ControllerBase {

        private readonly DataContext _context;

        public RecordController(DataContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? compid, string? tenant, string? id) {
            if (compid == null || tenant == null || id == null) return Conflict("parameters missing");

            string uri = $"https://itb2204.bc365.eu:7048/bc/api/trackers/tracking/v2.0/companies({compid})/salesOrders({id})/?tenant={tenant}";

            var users = _context.Users!.AsQueryable();
            var user = users.FirstOrDefault(x => x.Tenant == tenant);
            if (user?.Tenant == null) return NotFound($"User not found: {tenant}");

            using (var client = new HttpClient()) {
                var byteArray = Encoding.ASCII.GetBytes($"{user.Username}:{user.Key}");
                var header = new AuthenticationHeaderValue(
                       "Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = header;

                return Ok(await client.GetFromJsonAsync<Record>(uri));
            }
        }

        private int GetOrganizationId() {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return int.Parse(identity!.FindFirst("organizationId")!.Value);
        }
    }
}