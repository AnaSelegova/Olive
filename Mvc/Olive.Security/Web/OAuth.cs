﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Olive.Security
{
    public class OAuth
    {
        public readonly static OAuth Instance = new OAuth();

        public readonly AsyncEvent<ExternalLoginInfo> ExternalLoginAuthenticated = new AsyncEvent<ExternalLoginInfo>();

        public async Task LogOff()
        {
            await Context.Current.Http()?.SignOutAsync();
            Context.Current.Http()?.Session?.Clear();
        }

        public async Task LoginBy(string provider)
        {
            if (Context.Current.Request().Param("ReturnUrl").IsEmpty())
            {
                // it's mandatory, otherwise Challenge() immediately returns to Login page
                throw new InvalidOperationException("Request has no ReturnUrl.");
            }

            await Context.Current.Http().ChallengeAsync(provider, new AuthenticationProperties
            {
                RedirectUri = "/ExternalLoginCallback",
                Items = { new KeyValuePair<string, string>("LoginProvider", provider) }
            });
        }

        public async Task NotifyExternalLoginAuthenticated(ExternalLoginInfo info)
        {
            if (!ExternalLoginAuthenticated.IsHandled())
                throw new InvalidOperationException("ExternalLogin requested but no handler found for ExternalLoginAuthenticated event");

            await ExternalLoginAuthenticated.Raise(info);
        }

        public ClaimsPrincipal DecodeJwt(string jwt)
        {
            if (jwt.IsEmpty()) return null;

            var validationParams = new TokenValidationParameters()
            {
                IssuerSigningKey = GetJwtSecurityKey(),
                ValidateAudience = false,
                ValidateIssuer = false
            };

            return new JwtSecurityTokenHandler().ValidateToken(jwt, validationParams, out var token);
        }

        internal static SymmetricSecurityKey GetJwtSecurityKey()
        {
            var configKey = Config.GetOrThrow("Authentication:JWT:Secret");
            if (configKey.Length != 21)
                throw new ArgumentException("Your config setting of 'Authentication:JWT:Secret' needs to be 21 characters.");

            var securityKey = configKey.ToBytes(encoding: Encoding.UTF8);
            return new SymmetricSecurityKey(securityKey);
        }
    }
}