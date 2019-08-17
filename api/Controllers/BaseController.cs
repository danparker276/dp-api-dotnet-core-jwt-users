using dp.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dp.api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ClaimedUser GetClaimedUser()
        {
            bool isAdmin = User.IsInRole("Admin");
            //use something like ClaimTypes.NameIdentifier to put a value somewhere
            //example
           /* int? tenantId = int.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out int parsedTenantId)
                ? parsedTenantId
                : new int?();*/

            var currentUserId = int.Parse(User.Identity.Name);
            return new ClaimedUser
            {
                Id = currentUserId,
                IsAdmin = isAdmin
            };
        }
    }
}