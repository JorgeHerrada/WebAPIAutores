using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebAPIAutores.Tests.Mocks
{
    public class AuthorizationServiceMock : IAuthorizationService
    {
        public AuthorizationResult DesiredResult { get; set; }

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user, 
            object? resource, 
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(DesiredResult); // just return success, no real validation
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(DesiredResult); // just return success, no real validation
        }
    }
}
