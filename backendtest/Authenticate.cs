﻿/// <summary>
/// This file contains the <c>Authenticate</c> class, which is used for unit testing.
/// It provides a helper method to set a user token by adding a "UserId" claim to a controller's HTTP context.
/// This ensures that controller actions requiring authentication can be tested effectively.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backendtest
{
    /// <summary>
    /// The <c>Authenticate</c> class provides authentication utilities for unit tests.
    /// It enables the simulation of an authenticated user by setting a "UserId" claim 
    /// within a controller's HTTP context.
    /// </summary>
    public class Authenticate
    {
        /// <summary>
        /// Sets a "UserId" claim in the provided controller's HTTP context.
        /// This method is useful for unit tests that require authentication by 
        /// simulating an authenticated user with a specific ID.
        /// </summary>
        /// <param name="userId">The user ID to be set as a claim.</param>
        /// <param name="controller">The controller in which the claim should be set.</param>
        public static void SetUserIdClaim(string userId, ControllerBase controller)
        {
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }
    }
}
