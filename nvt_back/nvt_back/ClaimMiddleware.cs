using System.Data;
using System.Security.Claims;

namespace nvt_back
{
    public class ClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity is ClaimsIdentity identity)
            {
                try
                {
                    User loggedUser = new User();
                    loggedUser.Id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    loggedUser.Name = identity.FindFirst(ClaimTypes.Name)?.Value;
                    loggedUser.Email = identity.FindFirst(ClaimTypes.Email)?.Value;
                    loggedUser.Role = Enum.Parse<UserRole>(identity.FindFirst(ClaimTypes.Role)?.Value);
                    loggedUser.IsActivated = bool.Parse(identity.FindFirst(ClaimTypes.Version)?.Value);
                    context.Items["loggedUser"] = loggedUser;
                }
                catch
                {
                }
            }

            await _next(context);
        }

    }

}
