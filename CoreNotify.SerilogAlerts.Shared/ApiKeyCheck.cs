using Microsoft.AspNetCore.Authorization;

namespace CoreNotify.SerilogAlerts.Shared;

public class ApiKeyCheck(IOptions<>) : IAuthorizationRequirement
{
}
