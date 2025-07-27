using API.Services;
using System.IdentityModel.Tokens.Jwt;

namespace API.Middlewares
{
    public class ConnectionStringMiddleware
    {
        private readonly RequestDelegate _next;

        public ConnectionStringMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserDatabaseService userDbService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var connectionString = jwtToken.Claims.FirstOrDefault(c => c.Type == "ConnectionString")?.Value;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    // ✅ Store connection string in Scoped service
                    userDbService.SetConnectionString(connectionString);
                }
            }

            await _next(context);
        }
    }


}
