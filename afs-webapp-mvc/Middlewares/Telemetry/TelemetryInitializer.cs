using System.Security.Claims;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace BCHousing.Hiss.Web.Telemetry
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        private readonly IWebHostEnvironment _env;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public TelemetryInitializer(IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var environment = _env.IsDevelopment() ? "Dev" : "Production";
            telemetry.Context.Cloud.RoleName = $"{typeof(Program).Assembly.GetName().Name}-{environment}";

            var user = _httpContextAccessor?.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                telemetry.Context.User.Id = userId;
                telemetry.Context.GlobalProperties["user_name "] = user.Identity.Name ?? "NoNameUser";
                telemetry.Context.Session.Id = userId;
            }
            else
            {
                telemetry.Context.GlobalProperties["user_name "] = "Anonymous";
            }
        }
    }
}