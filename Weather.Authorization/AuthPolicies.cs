using Microsoft.AspNetCore.Authorization;

namespace ImageGalary.Authorization;

public static class AuthPolicies
{
    public static AuthorizationPolicy CanGetWeather()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("country", "srb")
            .RequireRole("admin")
            .Build();
    }
} 