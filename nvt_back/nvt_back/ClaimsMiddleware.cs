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
            Console.WriteLine("Middleware Start");
            if (context.User.Identity is ClaimsIdentity identity)
            {
                Console.WriteLine("Middleware sdsadsad");
                /*try
                {
                    User loggedUser = new User();
                    loggedUser.Id = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    loggedUser.Name = identity.FindFirst(ClaimTypes.Name)?.Value;
                    loggedUser.Email = identity.FindFirst(ClaimTypes.Email)?.Value;
                    Console.WriteLine(identity.FindFirst(identity.FindFirst(ClaimTypes.Role)?.Value));
                    loggedUser.Role = Enum.Parse<UserRole>(identity.FindFirst(ClaimTypes.Role)?.Value);
                    Console.WriteLine(loggedUser.Role);
                    loggedUser.IsActivated = true;
                    context.Items["loggedUser"] = loggedUser;

                    Console.WriteLine("Middleware user");
                    Console.WriteLine(context.Items["loggedUser"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in middleware: {ex}");
                }*/
            }

            await _next(context);
        }

    }

}
