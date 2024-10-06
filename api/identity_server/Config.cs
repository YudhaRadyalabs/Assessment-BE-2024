// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer4.Models;

namespace identity_server
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("assessment.api", "Assessment API")
                {
                    Scopes = { "assessment.api" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[] {
                new ApiScope("assessment.api", "assessment API"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "assessment_client",
                    ClientName = "",
                    AccessTokenLifetime = 60 * 60 * 24,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedScopes = { "openid", "profile", "email" },
                    // Allow the client to revoke tokens
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                }
            };
    }
}